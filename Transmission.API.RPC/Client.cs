using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Transmission.API.RPC.Entity;
using Newtonsoft.Json.Linq;
using Transmission.API.RPC.Common;
using Transmission.API.RPC.Arguments;

namespace Transmission.API.RPC
{
    /// <summary>
    /// Transmission client
    /// </summary>
    public partial class Client : ITransmissionClient, ITransmissionClientAsync
    {
        private readonly string _authorization;
        private readonly bool _needAuthorization;

        /// <summary>
        /// Url to service
        /// </summary>
        public string Url
        {
            get;
            private set;
        }

        /// <summary>
        /// Session ID
        /// </summary>
        public string SessionID
        {
            get;
            private set;
        }

        /// <summary>
        /// Current Tag
        /// </summary>
        public int CurrentTag
        {
            get;
            private set;
        }

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
        {
            this.Url = url;
            this.SessionID = sessionID;

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
        /// Close current session (API: session-close)
        /// </summary>
        public void CloseSession()
        {
            var request = new TransmissionRequest("session-close");
            var response = SendRequest(request);
        }

        /// <summary>
        /// Set information to current session (API: session-set)
        /// </summary>
        /// <param name="settings">New session settings</param>
        public void SetSessionSettings(SessionSettings settings)
        {
            var request = new TransmissionRequest("session-set", settings);
            var response = SendRequest(request);
        }

        /// <summary>
        /// Get session stat
        /// </summary>
        /// <returns>Session stat</returns>
        public Statistic GetSessionStatistic()
        {
            var request = new TransmissionRequest("session-stats");
            var response = SendRequest(request);
            var result = response.Deserialize<Statistic>();
            return result;
        }

        /// <summary>
        /// Get information of current session (API: session-get)
        /// </summary>
        /// <returns>Session information</returns>
        public SessionInfo GetSessionInformation()
        {
            var request = new TransmissionRequest("session-get");
            var response = SendRequest(request);
            var result = response.Deserialize<SessionInfo>();
            return result;
        }

        #endregion

        #region Torrents methods

        /// <summary>
        /// Add torrent (API: torrent-add)
        /// </summary>
        /// <returns>Torrent info (ID, Name and HashString)</returns>
		public NewTorrentInfo TorrentAdd(NewTorrent torrent)
        {
            if (String.IsNullOrWhiteSpace(torrent.Metainfo) && String.IsNullOrWhiteSpace(torrent.Filename))
                throw new Exception("Either \"filename\" or \"metainfo\" must be included.");

            var request = new TransmissionRequest("torrent-add", torrent);
            var response = SendRequest(request);
            var jObject = response.Deserialize<JObject>();

            if (jObject == null || jObject.First == null)
                return null;

            NewTorrentInfo result = null;
            JToken value = null;

            if (jObject.TryGetValue("torrent-duplicate", out value))
                result = JsonConvert.DeserializeObject<NewTorrentInfo>(value.ToString());
            else if (jObject.TryGetValue("torrent-added", out value))
                result = JsonConvert.DeserializeObject<NewTorrentInfo>(value.ToString());

            return result;
        }

        /// <summary>
        /// Set torrent params (API: torrent-set)
        /// </summary>
        /// <param name="settings">Torrent settings</param>
        public void TorrentSet(TorrentSettings settings)
        {
            var request = new TransmissionRequest("torrent-set", settings);
            var response = SendRequest(request);
        }

        /// <summary>
        /// Get fields of torrents from ids (API: torrent-get)
        /// </summary>
        /// <param name="fields">Fields of torrents</param>
        /// <param name="ids">IDs of torrents (null or empty for get all torrents)</param>
        /// <returns>Torrents info</returns>
        public TransmissionTorrents TorrentGet(string[] fields, params int[] ids)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("fields", fields);

            if (ids != null && ids.Length > 0)
                arguments.Add("ids", ids);

            var request = new TransmissionRequest("torrent-get", arguments);

            var response = SendRequest(request);
            var result = response.Deserialize<TransmissionTorrents>();

            return result;
        }

        /// <summary>
        /// Remove torrents (API: torrent-remove)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        /// <param name="deleteData">Remove data</param>
        public void TorrentRemove(int[] ids, bool deleteData = false)
        {
            var arguments = new Dictionary<string, object>();

            arguments.Add("ids", ids);
            arguments.Add("delete-local-data", deleteData);

            var request = new TransmissionRequest("torrent-remove", arguments);
            var response = SendRequest(request);
        }

        #region Torrent Start
        /// <summary>
        /// Start torrents (API: torrent-start)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        public void TorrentStart(object[] ids)
        {
            var request = new TransmissionRequest("torrent-start", new Dictionary<string, object> { { "ids", ids } });
            var response = SendRequest(request);
        }

        /// <summary>
        /// Start recently active torrents (API: torrent-start)
        /// </summary>
        public void TorrentStart()
        {
            var request = new TransmissionRequest("torrent-start", new Dictionary<string, object> { { "ids", "recently-active" } });
            var response = SendRequest(request);
        }
        #endregion

        #region Torrent Start Now

        /// <summary>
        /// Start now torrents (API: torrent-start-now)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        public void TorrentStartNow(object[] ids)
        {
            var request = new TransmissionRequest("torrent-start-now", new Dictionary<string, object> { { "ids", ids } });
            var response = SendRequest(request);
        }

        /// <summary>
        /// Start now recently active torrents (API: torrent-start-now)
        /// </summary>
        public void TorrentStartNow()
        {
            var request = new TransmissionRequest("torrent-start-now", new Dictionary<string, object> { { "ids", "recently-active" } });
            var response = SendRequest(request);
        }
        #endregion

        #region Torrent Stop
        /// <summary>
        /// Stop torrents (API: torrent-stop)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        public void TorrentStop(object[] ids)
        {
            var request = new TransmissionRequest("torrent-stop", new Dictionary<string, object> { { "ids", ids } });
            var response = SendRequest(request);
        }

        /// <summary>
        /// Stop recently active torrents (API: torrent-stop)
        /// </summary>
        public void TorrentStop()
        {
            var request = new TransmissionRequest("torrent-stop", new Dictionary<string, object> { { "ids", "recently-active" } });
            var response = SendRequest(request);
        }
        #endregion

        #region Torrent Verify
        /// <summary>
        /// Verify torrents (API: torrent-verify)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        public void TorrentVerify(object[] ids)
        {
            var request = new TransmissionRequest("torrent-verify", new Dictionary<string, object> { { "ids", ids } });
            var response = SendRequest(request);
        }

        /// <summary>
        /// Verify recently active torrents (API: torrent-verify)
        /// </summary>
        public void TorrentVerify()
        {
            var request = new TransmissionRequest("torrent-verify", new Dictionary<string, object> { { "ids", "recently-active" } });
            var response = SendRequest(request);
        }
        #endregion

        /// <summary>
        /// Move torrents in queue on top (API: queue-move-top)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void TorrentQueueMoveTop(int[] ids)
        {
            var request = new TransmissionRequest("queue-move-top", new Dictionary<string, object> { { "ids", ids } });
            var response = SendRequest(request);
        }

        /// <summary>
        /// Move up torrents in queue (API: queue-move-up)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentQueueMoveUp(int[] ids)
        {
            var request = new TransmissionRequest("queue-move-up", new Dictionary<string, object> { { "ids", ids } });
            var response = SendRequest(request);
        }

        /// <summary>
        /// Move down torrents in queue (API: queue-move-down)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentQueueMoveDown(int[] ids)
        {
            var request = new TransmissionRequest("queue-move-down", new Dictionary<string, object> { { "ids", ids } });
            var response = SendRequest(request);
        }

        /// <summary>
        /// Move torrents to bottom in queue  (API: queue-move-bottom)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentQueueMoveBottom(int[] ids)
        {
            var request = new TransmissionRequest("queue-move-bottom", new Dictionary<string, object> { { "ids", ids } });
            var response = SendRequest(request);
        }

        /// <summary>
        /// Set new location for torrents files (API: torrent-set-location)
        /// </summary>
        /// <param name="ids">Torrent ids</param>
        /// <param name="location">The new torrent location</param>
        /// <param name="move">Move from previous location</param>
        public void TorrentSetLocation(int[] ids, string location, bool move)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("ids", ids);
            arguments.Add("location", location);
            arguments.Add("move", move);

            var request = new TransmissionRequest("torrent-set-location", arguments);
            var response = SendRequest(request);
        }

        /// <summary>
        /// Rename a file or directory in a torrent (API: torrent-rename-path)
        /// </summary>
        /// <param name="id">The torrent whose path will be renamed</param>
        /// <param name="path">The path to the file or folder that will be renamed</param>
        /// <param name="name">The file or folder's new name</param>
		public RenameTorrentInfo TorrentRenamePath(int id, string path, string name)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("ids", new int[] { id });
            arguments.Add("path", path);
            arguments.Add("name", name);

            var request = new TransmissionRequest("torrent-rename-path", arguments);
            var response = SendRequest(request);

            var result = response.Deserialize<RenameTorrentInfo>();

            return result;
        }

        //method name not recognized
        ///// <summary>
        ///// Reannounce torrent (API: torrent-reannounce)
        ///// </summary>
        ///// <param name="ids"></param>
        //public void ReannounceTorrents(object[] ids)
        //{
        //    var arguments = new Dictionary<string, object>();
        //    arguments.Add("ids", ids);

        //    var request = new TransmissionRequest("torrent-reannounce", arguments);
        //    var response = SendRequest(request);
        //}

        #endregion

        #region System
        /// <summary>
        /// See if your incoming peer port is accessible from the outside world (API: port-test)
        /// </summary>
        /// <returns>Accessible state</returns>
        public bool PortTest()
        {
            var request = new TransmissionRequest("port-test");
            var response = SendRequest(request);

            var data = response.Deserialize<JObject>();
            var result = (bool)data.GetValue("port-is-open");
            return result;
        }

        /// <summary>
        /// Update blocklist (API: blocklist-update)
        /// </summary>
        /// <returns>Blocklist size</returns>
        public int BlocklistUpdate()
        {
            var request = new TransmissionRequest("blocklist-update");
            var response = SendRequest(request);

            var data = response.Deserialize<JObject>();
            var result = (int)data.GetValue("blocklist-size");
            return result;
        }

        /// <summary>
        /// Get free space is available in a client-specified folder.
        /// </summary>
        /// <param name="path">The directory to query</param>
        public long FreeSpace(string path)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("path", path);

            var request = new TransmissionRequest("free-space", arguments);
            var response = SendRequest(request);

            var data = response.Deserialize<JObject>();
            var result = (long)data.GetValue("size-bytes");
            return result;
        }
        #endregion

        private TransmissionResponse SendRequest(TransmissionRequest request)
        {
            TransmissionResponse result = new TransmissionResponse();

            request.Tag = ++CurrentTag;

            try
            {

                byte[] byteArray = Encoding.UTF8.GetBytes(request.ToJson());

                //Prepare http web request
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);

                webRequest.ContentType = "application/json-rpc";
                webRequest.Headers["X-Transmission-Session-Id"] = SessionID;
                webRequest.Method = "POST";

                if (_needAuthorization)
                    webRequest.Headers["Authorization"] = _authorization;

                var requestTask = webRequest.GetRequestStreamAsync();
                requestTask.WaitAndUnwrapException();
                using (Stream dataStream = requestTask.Result)
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                var responseTask = webRequest.GetResponseAsync();
                responseTask.WaitAndUnwrapException();

                //Send request and prepare response
                using (var webResponse = responseTask.Result)
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        var responseString = reader.ReadToEnd();
                        result = JsonConvert.DeserializeObject<TransmissionResponse>(responseString);

                        if (result.Result != "success")
                            throw new Exception(result.Result);
                    }
                }
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Conflict)
                {
                    if (ex.Response.Headers.Count > 0)
                    {
                        //If session id expiried, try get session id and send request
                        SessionID = ex.Response.Headers["X-Transmission-Session-Id"];

                        if (SessionID == null)
                            throw new Exception("Session ID Error");

                        result = SendRequest(request);
                    }
                }
                else
                    throw ex;
            }

            return result;
        }
    }
}
