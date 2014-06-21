using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Transmission.API.RPC.Entity;
using Newtonsoft.Json.Linq;

namespace Transmission.API.RPC
{
    public class Client
    {
        public string Host;

        public string SessionID;

        /// <summary>
        /// Get current session (API: session-get)
        /// </summary>
        /// <returns>Session info</returns>
        public TransmissionSession GetSession()
        {
            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "session-get",
                Tag = 0,
            };

            var response = sendRequest(request);
            var result = deserializeArguments<TransmissionSession>(response.Arguments);

            return result;
        }

        /// <summary>
        /// Get session stat
        /// </summary>
        /// <returns>Session stats</returns>
        public TransmissionSessionStat GetSessionStat()
        {
            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "session-stats",
                Tag = 0,
            };

            var response = sendRequest(request);
            var result = deserializeArguments<TransmissionSessionStat>(response.Arguments);
            return result;
        }

        /// <summary>
        /// Close current session (API: session-close)
        /// </summary>
        public void CloseSession()
        {
            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "session-close",
                Tag = 0,
            };

            sendRequest(request);
        }

        /// <summary>
        /// See if your incoming peer port is accessible from the outside world (API: port-test)
        /// </summary>
        /// <returns>Accessible state</returns>
        public bool CheckPort()
        {
            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "port-test",
                Tag = 0,
            };

            var response = sendRequest(request);

            var jObject = deserializeArguments<JObject>(response.Arguments);
            return (bool)jObject.GetValue("port-is-open"); ;
        }

        /// <summary>
        /// Add torrent (API: torrent-add)
        /// </summary>
        /// <returns>Torrent info (ID, Name and HashString)</returns>
        public TransmissionTorrent AddTorrent(TransmissionNewTorrent torrent)
        {
            TransmissionRequest request = new TransmissionRequest
            {
                Method = "torrent-add",
                Arguments = torrent.ToArguments(),
                Tag = 0,
            };

            var response = sendRequest(request);
            var jObject = deserializeArguments<JObject>(response.Arguments);

            if (jObject == null || jObject.First == null)
                return null;

            TransmissionTorrent result = null;
            JToken value = null;

            if (jObject.TryGetValue("torrent-duplicate", out value))
                result = JsonConvert.DeserializeObject<TransmissionTorrent>(value.ToString());
            else if (jObject.TryGetValue("torrent-added", out value))
                result = JsonConvert.DeserializeObject<TransmissionTorrent>(value.ToString());

            return result;
        }

        /// <summary>
        /// [UNIMPLEMENTED] Set torrent params (API: torrent-set)
        /// </summary>
        /// <param name="torrentSet">New torrent params</param>
        public void SetTorrents(TransmissionTorrentsSet torrentSet)
        {
            var requestArguments = new Dictionary<string, object>();
            requestArguments.Add("ids", torrentSet.IDs);
            requestArguments.Add("location", torrentSet.Location);
            //<...>

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "torrent-set",
                Arguments = requestArguments,
                Tag = 0,
            };

            sendRequest(request);
        }

        /// <summary>
        /// Get fields of torrents from ids (API: torrent-get)
        /// </summary>
        /// <param name="fields">Fields of torrents</param>
        /// <param name="ids">IDs of torrents (null or empty for get all torrents)</param>
        /// <returns>Torrents info</returns>
        public TransmissionTorrents GetTorrents(string[] fields, int[] ids = null)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("fields", fields);

            if (ids != null && ids.Length > 0)
                arguments.Add("ids", ids);

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "torrent-get",
                Arguments = arguments,
                Tag = 0,
            };

            var response = sendRequest(request);
            var result = deserializeArguments<TransmissionTorrents>(response.Arguments);

            return result;
        }

        /// <summary>
        /// Remove torrents
        /// </summary>
        /// <param name="ids">Torrents id</param>
        /// <param name="deleteLocalData">Remove local data</param>
        public void RemoveTorrents(int[] ids, bool deleteLocalData = false)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("ids", ids);
            args.Add("delete-local-data", deleteLocalData);

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "torrent-remove",
                Arguments = args,
                Tag = 0,
            };

            sendRequest(request);
        }

        /// <summary>
        /// Start torrents (API: torrent-start)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void StartTorrents(int[] ids)
        {
            var requestArguments = new Dictionary<string, object>();
            requestArguments.Add("ids", ids);

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "torrent-start",
                Arguments = requestArguments,
                Tag = 0,
            };

            sendRequest(request);
        }

        /// <summary>
        /// Start now torrents (API: torrent-start-now)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void StartNowTorrents(int[] ids)
        {
            var requestArguments = new Dictionary<string, object>();
            requestArguments.Add("ids", ids);

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "torrent-start-now",
                Arguments = requestArguments,
                Tag = 0,
            };

            sendRequest(request);
        }

        /// <summary>
        /// Stop torrents (API: torrent-stop)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void StopTorrents(int[] ids)
        {
            var requestArguments = new Dictionary<string, object>();
            requestArguments.Add("ids", ids);

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "torrent-stop",
                Arguments = requestArguments,
                Tag = 0,
            };

            sendRequest(request);
        }

        /// <summary>
        /// Verify torrents (API: torrent-verify)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void VerifyTorrents(int[] ids)
        {
            var requestArguments = new Dictionary<string, object>();
            requestArguments.Add("ids", ids);

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "torrent-verify",
                Arguments = requestArguments,
                Tag = 0,
            };

            sendRequest(request);
        }

        /// <summary>
        /// Move torrents in queue on top (API: queue-move-top)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void TorrentsQueueMoveTop(int[] ids)
        {
            var requestArguments = new Dictionary<string, object>();
            requestArguments.Add("ids", ids);

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "queue-move-top",
                Arguments = requestArguments,
                Tag = 0,
            };

            sendRequest(request);
        }

        /// <summary>
        /// Move up torrents in queue (API: queue-move-up)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentsQueueMoveUp(int[] ids)
        {
            var requestArguments = new Dictionary<string, object>();
            requestArguments.Add("ids", ids);

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "queue-move-up",
                Arguments = requestArguments,
                Tag = 0,
            };

            sendRequest(request);
        }

        /// <summary>
        /// Move down torrents in queue (API: queue-move-down)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentsQueueMoveDown(int[] ids)
        {
            var requestArguments = new Dictionary<string, object>();
            requestArguments.Add("ids", ids);

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "queue-move-down",
                Arguments = requestArguments,
                Tag = 0,
            };

            sendRequest(request);
        }

        /// <summary>
        /// Move torrents to bottom in queue  (API: queue-move-bottom)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentsQueueMoveBottom(int[] ids)
        {
            var requestArguments = new Dictionary<string, object>();
            requestArguments.Add("ids", ids);

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "queue-move-bottom",
                Arguments = requestArguments,
                Tag = 0,
            };

            sendRequest(request);
        }

        /// <summary>
        /// Set new location for torrents files (API: torrent-set-location)
        /// </summary>
        /// <param name="ids">Torrent ids</param>
        /// <param name="location">The new torrent location</param>
        /// <param name="move">Move from previous location</param>
        public void SetLocationTorrents(int[] ids, string location, bool move)
        {
            var requestArguments = new Dictionary<string, object>();
            requestArguments.Add("ids", ids);
            requestArguments.Add("location", location);
            requestArguments.Add("move", move);

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "torrent-set-location",
                Arguments = requestArguments,
                Tag = 0,
            };

            sendRequest(request);
        }

        /// <summary>
        /// [UNTESTED!] Rename a file or directory in a torrent (API: torrent-rename-path)
        /// </summary>
        /// <param name="ids">The torrent whose path will be renamed</param>
        /// <param name="path">The path to the file or folder that will be renamed</param>
        /// <param name="name">The file or folder's new name</param>
        public void RenameTorrentPath(int id, string path, string name)
        {
            var requestArguments = new Dictionary<string, object>();
            requestArguments.Add("ids", new int[] { id });
            requestArguments.Add("path", path);
            requestArguments.Add("name", name);

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "torrent-rename-path",
                Arguments = requestArguments,
                Tag = 0,
            };

            sendRequest(request);
        }

        /// <summary>
        /// Reannounce torrent (API: torrent-reannounce)
        /// </summary>
        /// <param name="ids"></param>
        public void ReannounceTorrents(int[] ids)
        {
            var requestArguments = new Dictionary<string, object>();
            requestArguments.Add("ids", ids);

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "torrent-reannounce",
                Arguments = requestArguments,
                Tag = 0,
            };

            sendRequest(request);
        }

        /// <summary>
        /// Update blocklist (API: blocklist-update)
        /// </summary>
        public int BlocklistUpdate()
        {

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "blocklist-update",
                Tag = 0,
            };

            var response = sendRequest(request);

            var result = 0;

            if (response.Result != "success")
                throw new Exception(response.Result);
            else
            {
                var jObject = deserializeArguments<JObject>(response.Arguments);
                result = (int)jObject.GetValue("blocklist-size");
            }

            return result;
        }

        /// <summary>
        /// Get free space is available in a client-specified folder.
        /// </summary>
        /// <param name="path">The directory to query</param>
        public TransmissionFreeSpace FreeSpace(string path)
        {
            var requestArguments = new Dictionary<string, object>();
            requestArguments.Add("path", path);

            TransmissionRequest request = new TransmissionRequest()
            {
                Method = "free-space",
                Arguments = requestArguments,
                Tag = 0,
            };

            var response = sendRequest(request);
            var result = deserializeArguments<TransmissionFreeSpace>(response.Arguments);
            return result;
        }

        #region Private

        private T deserializeArguments<T>(Dictionary<string, object> arguments)
        {
            var argumentsString = JsonConvert.SerializeObject(arguments);
            return JsonConvert.DeserializeObject<T>(argumentsString);
        }

        private TransmissionResponse sendRequest(TransmissionRequest data)
        {
            TransmissionResponse result = new TransmissionResponse();

            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(data.ToString());

                //Prepare http web request
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Host);
                webRequest.ContentType = "application/json-rpc";
                webRequest.Headers.Add("X-Transmission-Session-Id:" + SessionID);
                webRequest.Method = "POST";
                webRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                //Send request and prepare response
                using (var webResponse = webRequest.GetResponse())
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        var responseString = reader.ReadToEnd();
                        result = JsonConvert.DeserializeObject<TransmissionResponse>(responseString);
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null && ex.Response.Headers.HasKeys())
                {
                    //If session id expiried, try get session id and send request
                    SessionID = ex.Response.Headers.GetValues("X-Transmission-Session-Id").FirstOrDefault();
                    result = sendRequest(data);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        #endregion
    }

    //    Requests support three keys:
    //   (1) A required "method" string telling the name of the method to invoke
    //   (2) An optional "arguments" object of key/value pairs
    //   (3) An optional "tag" number used by clients to track responses.
    //       If provided by a request, the response MUST include the same tag.
    public class TransmissionRequest
    {
        [JsonProperty("method")]
        public string Method;

        [JsonProperty("arguments")]
        public Dictionary<string, object> Arguments;

        [JsonProperty("tag")]
        public int Tag;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    //   Reponses support three keys:
    //   (1) A required "result" string whose value MUST be "success" on success,
    //       or an error string on failure.
    //   (2) An optional "arguments" object of key/value pairs
    //   (3) An optional "tag" number as described in 2.1.
    public class TransmissionResponse
    {
        [JsonProperty("result")]
        public string Result;

        [JsonProperty("arguments")]
        public Dictionary<string, object> Arguments;

        [JsonProperty("tag")]
        public int Tag;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

}
