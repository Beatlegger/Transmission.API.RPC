using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Transmission.API.RPC.Arguments;
using Transmission.API.RPC.Common;
using Transmission.API.RPC.Entity;

namespace Transmission.API.RPC
{
    /// <summary>
    /// Transmission client
    /// </summary>
    public partial class Client : ITransmissionClient
    {
        /// <summary>
        /// Authorization header value for requests
        /// </summary>
        private readonly string _authorization;

        /// <summary>
        /// Need authorization for requests
        /// </summary>
        private readonly bool _needAuthorization;

        /// <summary>
        /// HttpClient instance for making HTTP requests
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Lock object for thread-safe SessionID updates
        /// </summary>
        private readonly object _sessionLock = new object();

        /// <summary>
        /// Lock object for thread-safe tag increments
        /// </summary>
        private readonly object _tagLock = new object();

        /// <summary>
        /// Maximum number of retries for a request when SessionID is invalid
        /// </summary>
        private const int MaxSessionRetries = 3;

        /// <summary>
        /// Url to service
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Session ID
        /// </summary>
        public string SessionID { get; private set; }

        /// <summary>
        /// Current Tag
        /// </summary>
        public int CurrentTag { get; private set; }

        /// <summary>
        /// Initialize client
        /// <example>For example
        /// <code>
        /// new Transmission.API.RPC.Client("https://website.com:9091/transmission/rpc")
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="url">URL to Transmission RPC API. Often it looks like schema://host:port/transmission/rpc </param>
        /// <param name="sessionID">Session ID</param>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        public Client(string url, string sessionID = null, string login = null, string password = null)
            : this(url, null, sessionID, login, password)
        {
        }

        /// <summary>
        /// Initialize client with custom HttpClient
        /// </summary>
        /// <param name="url">URL to Transmission RPC API</param>
        /// <param name="httpClient">Custom HttpClient instance (if null, a new instance is created)</param>
        /// <param name="sessionID">Session ID</param>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        public Client(string url, HttpClient httpClient, string sessionID = null, string login = null, string password = null)
        {
            this.Url = url;
            this.SessionID = sessionID;
            // When no HttpClient is supplied, build one with a short pooled-connection
            // lifetime. Transmission aggressively closes keep-alive connections; without
            // this, HttpClient's default pool hands out half-dead sockets that fail
            // mid-response with HttpIOException: ResponseEnded under rapid back-to-back
            // requests (e.g. integration tests).
            this._httpClient = httpClient ?? new HttpClient(new SocketsHttpHandler
            {
                // Zero disables connection pooling entirely — each request uses a
                // fresh TCP connection, avoiding stale-socket ResponseEnded errors.
                PooledConnectionLifetime = TimeSpan.Zero,
            });

            if (!String.IsNullOrWhiteSpace(login))
            {
                var authBytes = Encoding.UTF8.GetBytes(login + ":" + password);
                var encoded = Convert.ToBase64String(authBytes);

                this._authorization = "Basic " + encoded;
                this._needAuthorization = true;
            }
        }

        #region Session methods

        /// <summary>
        /// Sends the <c>session-close</c> RPC (API spec §4.5).
        /// <para>
        /// The spec describes this method as "tells the Transmission session
        /// to shut down", but empirically on Transmission 4.1.1 it behaves as
        /// a no-op: the daemon keeps running, the CSRF
        /// <c>X-Transmission-Session-Id</c> is not rotated, and subsequent
        /// RPC requests continue to succeed on the same client. Older
        /// Transmission versions may have actually exited the daemon — behavior
        /// here depends on the server implementation.
        /// </para>
        /// <para>
        /// To invalidate the cached <c>X-Transmission-Session-Id</c> CSRF
        /// token on the client (forcing a fresh handshake on the next
        /// request), use <see cref="ResetSessionId"/>.
        /// </para>
        /// </summary>
        public async Task CloseSessionAsync()
        {
            var request = new TransmissionRequest("session-close");
            var response = await SendRequestAsync(request);
            // The daemon invalidates the server-side session on session-close.
            // Subsequent requests reusing the cached CSRF token can return stale
            // or empty results — force a fresh handshake on the next call.
            ResetSessionId();
        }

        /// <summary>
        /// Clears the cached <c>X-Transmission-Session-Id</c> CSRF token on
        /// the client so that the next RPC request performs a fresh handshake
        /// via the 409 retry flow.
        /// <para>
        /// This is a purely client-side operation — it sends no RPC and does
        /// not affect the running daemon. Use it when you want to force a new
        /// handshake (e.g. after credentials change or to recover from a
        /// stale token) without shutting down the daemon the way
        /// <see cref="CloseSessionAsync"/> does.
        /// </para>
        /// </summary>
        public void ResetSessionId()
        {
            lock (_sessionLock)
            {
                SessionID = null;
            }
        }

        /// <summary>
        /// Set information to current session (API: session-set)
        /// </summary>
        /// <param name="settings">New session settings</param>
        public async Task SetSessionSettingsAsync(SessionSettings settings)
        {
            var request = new TransmissionRequest("session-set", settings);
            var response = await SendRequestAsync(request);
        }

        /// <summary>
        /// Get session stat
        /// </summary>
        /// <returns>Session stat</returns>
        public async Task<Statistic> GetSessionStatisticAsync()
        {
            var request = new TransmissionRequest("session-stats");
            var response = await SendRequestAsync(request);
            var result = response.Deserialize(TransmissionJsonArgumentsContext.Default.Statistic);
            return result;
        }

        /// <summary>
        /// Get information of current session (API: session-get)
        /// </summary>
        /// <returns>Session information</returns>
        //TODO: support optional "fields" argument
        public async Task<SessionInfo> GetSessionInformationAsync()
        {
            var request = new TransmissionRequest("session-get");
            var response = await SendRequestAsync(request);
            var result = response.Deserialize(TransmissionJsonArgumentsContext.Default.SessionInfo);
            return result;
        }

        #endregion

        #region Torrents methods

        /// <summary>
        /// Add torrent (API: torrent-add)
        /// </summary>
        /// <returns>Torrent info (ID, Name and HashString)</returns>
        public async Task<NewTorrentInfo> TorrentAddAsync(NewTorrent torrent)
        {
            if (String.IsNullOrWhiteSpace(torrent.Metainfo) && String.IsNullOrWhiteSpace(torrent.Filename))
                throw new Exception("Either \"filename\" or \"metainfo\" must be included.");

            var request = new TransmissionRequest("torrent-add", torrent);
            var response = await SendRequestAsync(request);

            if (response.Arguments == null)
                return null;

            var json = JsonSerializer.Serialize(response.Arguments, TransmissionJsonArgumentsContext.Default.DictionaryStringObject);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            NewTorrentInfo result = null;

            if (root.TryGetProperty("torrent-duplicate", out var dupValue))
                result = JsonSerializer.Deserialize(dupValue.GetRawText(), TransmissionJsonArgumentsContext.Default.NewTorrentInfo);
            else if (root.TryGetProperty("torrent-added", out var addValue))
                result = JsonSerializer.Deserialize(addValue.GetRawText(), TransmissionJsonArgumentsContext.Default.NewTorrentInfo);

            return result;
        }

        /// <summary>
        /// Set torrent params (API: torrent-set)
        /// </summary>
        /// <param name="settings">Torrent settings</param>
        public async Task TorrentSetAsync(TorrentSettings settings)
        {
            var request = new TransmissionRequest("torrent-set", settings);
            var response = await SendRequestAsync(request);
        }

        /// <summary>
        /// Get fields of torrents from ids (API: torrent-get)
        /// </summary>
        /// <param name="fields">Fields of torrents</param>
        /// <param name="ids">IDs of torrents (null or empty for get all torrents)</param>
        /// <returns>Torrents info</returns>
        public async Task<TransmissionTorrents> TorrentGetAsync(string[] fields, params int[] ids)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("fields", fields);

            if (ids != null && ids.Length > 0)
                arguments.Add("ids", ids);

            var request = new TransmissionRequest("torrent-get", arguments);

            var response = await SendRequestAsync(request);
            var result = response.Deserialize(TransmissionJsonArgumentsContext.Default.TransmissionTorrents);

            return result;
        }

        /// <summary>
        /// Remove torrents
        /// </summary>
        /// <param name="ids">Torrents id</param>
        /// <param name="deleteData">Remove data</param>
        public async Task TorrentRemoveAsync(int[] ids, bool deleteData = false)
        {
            var arguments = new Dictionary<string, object>();

            arguments.Add("ids", ids);
            arguments.Add("delete-local-data", deleteData);

            var request = new TransmissionRequest("torrent-remove", arguments);
            var response = await SendRequestAsync(request);
        }

        #region Torrent Start

        /// <summary>
        /// Start torrents (API: torrent-start)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        public async Task TorrentStartAsync(object[] ids)
        {
            var request = new TransmissionRequest("torrent-start", new Dictionary<string, object> { { "ids", ids } });
            var response = await SendRequestAsync(request);
        }

        /// <summary>
        /// Start recently active torrents (API: torrent-start)
        /// </summary>
        public async Task TorrentStartAsync()
        {
            var request = new TransmissionRequest("torrent-start", new Dictionary<string, object> { { "ids", "recently-active" } });
            var response = await SendRequestAsync(request);
        }

        #endregion

        #region Torrent Start Now

        /// <summary>
        /// Start now torrents (API: torrent-start-now)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        public async Task TorrentStartNowAsync(object[] ids)
        {
            var request = new TransmissionRequest("torrent-start-now", new Dictionary<string, object> { { "ids", ids } });
            var response = await SendRequestAsync(request);
        }

        /// <summary>
        /// Start now recently active torrents (API: torrent-start-now)
        /// </summary>
        public async Task TorrentStartNowAsync()
        {
            var request = new TransmissionRequest("torrent-start-now", new Dictionary<string, object> { { "ids", "recently-active" } });
            var response = await SendRequestAsync(request);
        }

        #endregion

        #region Torrent Stop

        /// <summary>
        /// Stop torrents (API: torrent-stop)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        public async Task TorrentStopAsync(object[] ids)
        {
            var request = new TransmissionRequest("torrent-stop", new Dictionary<string, object> { { "ids", ids } });
            var response = await SendRequestAsync(request);
        }

        /// <summary>
        /// Stop recently active torrents (API: torrent-stop)
        /// </summary>
        public async Task TorrentStopAsync()
        {
            var request = new TransmissionRequest("torrent-stop", new Dictionary<string, object> { { "ids", "recently-active" } });
            var response = await SendRequestAsync(request);
        }

        #endregion

        #region Torrent Verify

        /// <summary>
        /// Verify torrents (API: torrent-verify)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        public async Task TorrentVerifyAsync(object[] ids)
        {
            var request = new TransmissionRequest("torrent-verify", new Dictionary<string, object> { { "ids", ids } });
            var response = await SendRequestAsync(request);
        }

        /// <summary>
        /// Verify recently active torrents (API: torrent-verify)
        /// </summary>
        public async Task TorrentVerifyAsync()
        {
            var request = new TransmissionRequest("torrent-verify", new Dictionary<string, object> { { "ids", "recently-active" } });
            var response = await SendRequestAsync(request);
        }
        #endregion

        /// <summary>
        /// Move torrents in queue on top (API: queue-move-top)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public async Task TorrentQueueMoveTopAsync(int[] ids)
        {
            var request = new TransmissionRequest("queue-move-top", new Dictionary<string, object> { { "ids", ids } });
            var response = await SendRequestAsync(request);
        }

        /// <summary>
        /// Move up torrents in queue (API: queue-move-up)
        /// </summary>
        /// <param name="ids"></param>
        public async Task TorrentQueueMoveUpAsync(int[] ids)
        {
            var request = new TransmissionRequest("queue-move-up", new Dictionary<string, object> { { "ids", ids } });
            var response = await SendRequestAsync(request);
        }

        /// <summary>
        /// Move down torrents in queue (API: queue-move-down)
        /// </summary>
        /// <param name="ids"></param>
        public async Task TorrentQueueMoveDownAsync(int[] ids)
        {
            var request = new TransmissionRequest("queue-move-down", new Dictionary<string, object> { { "ids", ids } });
            var response = await SendRequestAsync(request);
        }

        /// <summary>
        /// Move torrents to bottom in queue  (API: queue-move-bottom)
        /// </summary>
        /// <param name="ids"></param>
        public async Task TorrentQueueMoveBottomAsync(int[] ids)
        {
            var request = new TransmissionRequest("queue-move-bottom", new Dictionary<string, object> { { "ids", ids } });
            var response = await SendRequestAsync(request);
        }

        /// <summary>
        /// Set new location for torrents files (API: torrent-set-location)
        /// </summary>
        /// <param name="ids">Torrent ids</param>
        /// <param name="location">The new torrent location</param>
        /// <param name="move">Move from previous location</param>
        public async Task TorrentSetLocationAsync(int[] ids, string location, bool move)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("ids", ids);
            arguments.Add("location", location);
            arguments.Add("move", move);

            var request = new TransmissionRequest("torrent-set-location", arguments);
            var response = await SendRequestAsync(request);
        }

        /// <summary>
        /// Rename a file or directory in a torrent (API: torrent-rename-path)
        /// </summary>
        /// <param name="id">The torrent whose path will be renamed</param>
        /// <param name="path">The path to the file or folder that will be renamed</param>
        /// <param name="name">The file or folder's new name</param>
        public async Task<RenameTorrentInfo> TorrentRenamePathAsync(int id, string path, string name)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("ids", new int[] { id });
            arguments.Add("path", path);
            arguments.Add("name", name);

            var request = new TransmissionRequest("torrent-rename-path", arguments);
            var response = await SendRequestAsync(request);

            var result = response.Deserialize(TransmissionJsonArgumentsContext.Default.RenameTorrentInfo);

            return result;
        }

        #endregion

        #region System

        /// <summary>
        /// See if your incoming peer port is accessible from the outside world (API: port-test)
        /// </summary>
        /// <returns>Accessible state</returns>
        public async Task<bool> PortTestAsync()
        {
            var request = new TransmissionRequest("port-test");
            var response = await SendRequestAsync(request);

            var json = JsonSerializer.Serialize(response.Arguments, TransmissionJsonArgumentsContext.Default.DictionaryStringObject);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("port-is-open").GetBoolean();
        }

        /// <summary>
        /// Update blocklist (API: blocklist-update)
        /// </summary>
        /// <returns>Blocklist size</returns>
        public async Task<int> BlocklistUpdateAsync()
        {
            var request = new TransmissionRequest("blocklist-update");
            var response = await SendRequestAsync(request);

            var json = JsonSerializer.Serialize(response.Arguments, TransmissionJsonArgumentsContext.Default.DictionaryStringObject);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("blocklist-size").GetInt32();
        }

        /// <summary>
        /// Get free space is available in a client-specified folder.
        /// </summary>
        /// <param name="path">The directory to query</param>
        public async Task<long> FreeSpaceAsync(string path)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("path", path);

            var request = new TransmissionRequest("free-space", arguments);
            var response = await SendRequestAsync(request);

            var json = JsonSerializer.Serialize(response.Arguments, TransmissionJsonArgumentsContext.Default.DictionaryStringObject);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("size-bytes").GetInt64();
        }

        #endregion

        private async Task<TransmissionResponse> SendRequestAsync(TransmissionRequest request)
        {
            int tag;
            lock (_tagLock)
            {
                tag = ++CurrentTag;
            }
            request.Tag = tag;

            // Higher retry count + longer backoff to survive temporary daemon
            // unresponsiveness (e.g. Transmission briefly drops connections while
            // a blocklist-update is in flight). Total wait ~7.5s across 5 retries.
            int maxRetries = 5;
            for (int attempt = 0; attempt <= MaxSessionRetries; attempt++)
            {
                string sessionId;
                lock (_sessionLock)
                {
                    sessionId = SessionID;
                }

                for (int retry = 0; retry <= maxRetries; retry++)
                {
                    try
                    {
                        // Create new HttpRequestMessage for each attempt (can't reuse)
                        var httpRequest = new HttpRequestMessage(HttpMethod.Post, Url);
                        httpRequest.Headers.Add("X-Transmission-Session-Id", sessionId);
                        // Force a fresh TCP connection each request. Transmission
                        // aggressively closes keep-alive connections, and HttpClient's
                        // connection pool otherwise hands out half-dead sockets that
                        // fail mid-response with HttpIOException: ResponseEnded.
                        httpRequest.Headers.ConnectionClose = true;

                        if (_needAuthorization)
                            httpRequest.Headers.Add("Authorization", _authorization);

                        httpRequest.Content = new StringContent(request.ToRpcJson(), Encoding.UTF8, "application/json-rpc");

                        using (var httpResponse = await _httpClient.SendAsync(httpRequest))
                        {
                            if (httpResponse.IsSuccessStatusCode)
                            {
                                var responseString = await httpResponse.Content.ReadAsStringAsync();
                                var result = JsonSerializer.Deserialize(responseString, TransmissionJsonResponseContext.Default.TransmissionResponse);

                                if (result.Result != "success")
                                    throw new Exception(result.Result);

                                return result;
                            }
                            else if (httpResponse.StatusCode == HttpStatusCode.Conflict)
                            {
                                if (httpResponse.Headers.TryGetValues("X-Transmission-Session-Id", out var values))
                                {
                                    var newSessionId = values.First();
                                    lock (_sessionLock)
                                    {
                                        SessionID = newSessionId;
                                    }
                                }
                                else
                                    throw new Exception("Session ID Error");

                                // Break inner retry loop to retry outer loop with new session ID
                                break;
                            }
                            else
                                throw new HttpRequestException();
                        }
                    }
                    catch (HttpRequestException) when (retry < maxRetries)
                    {
                        // Wait before retry with exponential backoff
                        await Task.Delay(TimeSpan.FromMilliseconds(250 * Math.Pow(2, retry)));
                    }
                    catch (IOException) when (retry < maxRetries)
                    {
                        // HttpIOException (ResponseEnded) can surface as a bare IOException
                        // from ReadAsStringAsync when the server closes mid-stream — retry.
                        await Task.Delay(TimeSpan.FromMilliseconds(250 * Math.Pow(2, retry)));
                    }
                }
            }

            throw new Exception($"Failed to obtain valid session ID after {MaxSessionRetries} retries");
        }
    }
}
