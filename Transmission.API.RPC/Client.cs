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
using Transmission.API.RPC.Common;
using Transmission.API.RPC.Arguments;

namespace Transmission.API.RPC
{
    public class Client
    {
		public string Host
		{
			get;
			private set;
		}
		public string SessionID
		{
			get;
			private set;
		}
		public int CurrentTag
		{
			get;
			private set;
		}
  
        private string _authorization;
        private bool _needAuthorization;

		/// <summary>
		/// Initialize client
		/// </summary>
		/// <param name="host">Host adresse</param>
		/// <param name="sessionID">Session ID</param>
		/// <param name="login">Login</param>
		/// <param name="password">Password</param>
		public Client(string host, string sessionID = null, string login = null, string password = null)
        {
            this.Host = host;
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
            var request = new TransmissionRequest("session-close", null);
            var response = SendRequest(request);
        }

        /// <summary>
        /// Set information to current session (API: session-set)
        /// </summary>
        /// <param name="settings">New session settings</param>
        public void SetSessionSettings(SessionSettings settings)
        {
            var request = new TransmissionRequest("session-set", settings.ToDictionary());
            var response = SendRequest(request);
        }

        /// <summary>
        /// Get session stat
        /// </summary>
        /// <returns>Session stat</returns>
        public Statistic GetSessionStatistic()
        {
            var request = new TransmissionRequest("session-stats", null);
            var response = SendRequest(request);
            var result = response.Deserialize<Statistic>();
            return result;
        }

        /// <summary>
        /// Get information of current session (API: session-get)
        /// </summary>
        /// <returns>Session information</returns>
        public SessionInformation GetSessionInformation()
        {
            var request = new TransmissionRequest("session-get", null);
            var response = SendRequest(request);
            var result = response.Deserialize<SessionInformation>();
            return result;
        }

        #endregion

        #region Torrents methods

        /// <summary>
        /// Add torrent (API: torrent-add)
        /// </summary>
        /// <returns>Torrent info (ID, Name and HashString)</returns>
		public NewTorrentInformation AddTorrent(NewTorrent torrent)
        {
            if (String.IsNullOrWhiteSpace(torrent.Metainfo) && String.IsNullOrWhiteSpace(torrent.Filename))
                throw new Exception("Either \"filename\" or \"metainfo\" must be included.");

            var request = new TransmissionRequest("torrent-add", torrent.ToDictionary());
            var response = SendRequest(request);
            var jObject = response.Deserialize<JObject>();

            if (jObject == null || jObject.First == null)
                return null;

            NewTorrentInformation result = null;
            JToken value = null;

            if (jObject.TryGetValue("torrent-duplicate", out value))
                result = JsonConvert.DeserializeObject<NewTorrentInformation>(value.ToString());
            else if (jObject.TryGetValue("torrent-added", out value))
                result = JsonConvert.DeserializeObject<NewTorrentInformation>(value.ToString());

            return result;
        }

        /// <summary>
        /// Set torrent params (API: torrent-set)
        /// </summary>
        /// <param name="torrentSet">New torrent params</param>
        public void SetTorrents(TorrentSettings settings)
        {
            var request = new TransmissionRequest("torrent-set", settings.ToDictionary());
            var response = SendRequest(request);
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

            var request = new TransmissionRequest("torrent-get", arguments);

            var response = SendRequest(request);
            var result = response.Deserialize<TransmissionTorrents>();

            return result;
        }

        /// <summary>
        /// Remove torrents
        /// </summary>
        /// <param name="ids">Torrents id</param>
        /// <param name="deleteLocalData">Remove local data</param>
        public void RemoveTorrents(int[] ids, bool deleteData = false)
        {
            var arguments = new Dictionary<string, object>();

            arguments.Add("ids", ids);
            arguments.Add("delete-local-data", deleteData);

            var request = new TransmissionRequest("torrent-remove", arguments);
            var response = SendRequest(request);
        }

        /// <summary>
        /// Start torrents (API: torrent-start)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void StartTorrents(int[] ids)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("ids", ids);

            var request = new TransmissionRequest("torrent-start", arguments);
            var response = SendRequest(request);
        }

        /// <summary>
        /// Start now torrents (API: torrent-start-now)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void StartNowTorrents(int[] ids)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("ids", ids);

            var request = new TransmissionRequest("torrent-start-now", arguments);
            var response = SendRequest(request);
        }

        /// <summary>
        /// Stop torrents (API: torrent-stop)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void StopTorrents(int[] ids)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("ids", ids);

            var request = new TransmissionRequest("torrent-stop", arguments);
            var response = SendRequest(request);
        }

        /// <summary>
        /// Verify torrents (API: torrent-verify)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void VerifyTorrents(int[] ids)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("ids", ids);

            var request = new TransmissionRequest("torrent-verify", arguments);
            var response = SendRequest(request);
        }

        /// <summary>
        /// Move torrents in queue on top (API: queue-move-top)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void TorrentsQueueMoveTop(int[] ids)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("ids", ids);

            var request = new TransmissionRequest("queue-move-top", arguments);
            var response = SendRequest(request);
        }

        /// <summary>
        /// Move up torrents in queue (API: queue-move-up)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentsQueueMoveUp(int[] ids)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("ids", ids);

            var request = new TransmissionRequest("queue-move-up", arguments);
            var response = SendRequest(request);
        }

        /// <summary>
        /// Move down torrents in queue (API: queue-move-down)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentsQueueMoveDown(int[] ids)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("ids", ids);

            var request = new TransmissionRequest("queue-move-down", arguments);
            var response = SendRequest(request);
        }

        /// <summary>
        /// Move torrents to bottom in queue  (API: queue-move-bottom)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentsQueueMoveBottom(int[] ids)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("ids", ids);

            var request = new TransmissionRequest("queue-move-bottom", arguments);
            var response = SendRequest(request);
        }

        /// <summary>
        /// Set new location for torrents files (API: torrent-set-location)
        /// </summary>
        /// <param name="ids">Torrent ids</param>
        /// <param name="location">The new torrent location</param>
        /// <param name="move">Move from previous location</param>
        public void SetLocationTorrents(int[] ids, string location, bool move)
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
        /// <param name="ids">The torrent whose path will be renamed</param>
        /// <param name="path">The path to the file or folder that will be renamed</param>
        /// <param name="name">The file or folder's new name</param>
        public void RenameTorrentPath(int id, string path, string name)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("ids", new int[] { id });
            arguments.Add("path", path);
            arguments.Add("name", name);

            var request = new TransmissionRequest("torrent-rename-path", arguments);
            var response = SendRequest(request);
        }

        /// <summary>
        /// Reannounce torrent (API: torrent-reannounce)
        /// </summary>
        /// <param name="ids"></param>
        public void ReannounceTorrents(int[] ids)
        {
            var arguments = new Dictionary<string, object>();
            arguments.Add("ids", ids);

            var request = new TransmissionRequest("torrent-reannounce", arguments);
            var response = SendRequest(request);
        }

        #endregion

        #region System

        /// <summary>
        /// See if your incoming peer port is accessible from the outside world (API: port-test)
        /// </summary>
        /// <returns>Accessible state</returns>
        public bool CheckPort()
        {
            var request = new TransmissionRequest("port-test", null);
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
            var request = new TransmissionRequest("blocklist-update", null);
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
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Host);

                webRequest.ContentType = "application/json-rpc";
                webRequest.Headers.Add("X-Transmission-Session-Id:" + SessionID);
                webRequest.Method = "POST";
                webRequest.ContentLength = byteArray.Length;

                if(_needAuthorization)
                    webRequest.Headers.Add("Authorization", _authorization);

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

                        if (result.Result != "success")
                            throw new Exception(result.Result);
                    }
                }
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Conflict)
                {
                    if (ex.Response.Headers.HasKeys())
                    {
                        //If session id expiried, try get session id and send request
                        SessionID = ex.Response.Headers.GetValues("X-Transmission-Session-Id").FirstOrDefault();

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
