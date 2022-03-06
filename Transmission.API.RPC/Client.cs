using System;
using System.Text;
using Transmission.API.RPC.Entity;
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
            CloseSessionAsync().WaitAndUnwrapException();
        }

        /// <summary>
        /// Set information to current session (API: session-set)
        /// </summary>
        /// <param name="settings">New session settings</param>
        public void SetSessionSettings(SessionSettings settings)
        {
            SetSessionSettingsAsync(settings).WaitAndUnwrapException();
        }

        /// <summary>
        /// Get session stat
        /// </summary>
        /// <returns>Session stat</returns>
        public Statistic GetSessionStatistic()
        {
            var task = GetSessionStatisticAsync();
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Get information of current session (API: session-get)
        /// </summary>
        /// <returns>Session information</returns>
        public SessionInfo GetSessionInformation()
        {
            var task = GetSessionInformationAsync();
            task.WaitAndUnwrapException();
            return task.Result;
        }

        #endregion

        #region Torrents methods

        /// <summary>
        /// Add torrent (API: torrent-add)
        /// </summary>
        /// <returns>Torrent info (ID, Name and HashString)</returns>
		public NewTorrentInfo TorrentAdd(NewTorrent torrent)
        {
            var task = TorrentAddAsync(torrent);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Set torrent params (API: torrent-set)
        /// </summary>
        /// <param name="settings">Torrent settings</param>
        public void TorrentSet(TorrentSettings settings)
        {
            TorrentSetAsync(settings).WaitAndUnwrapException();
        }

        /// <summary>
        /// Get fields of torrents from ids (API: torrent-get)
        /// </summary>
        /// <param name="fields">Fields of torrents</param>
        /// <param name="ids">IDs of torrents (null or empty for get all torrents)</param>
        /// <returns>Torrents info</returns>
        public TransmissionTorrents TorrentGet(string[] fields, params int[] ids)
        {
            var task = TorrentGetAsync(fields, ids);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Remove torrents (API: torrent-remove)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        /// <param name="deleteData">Remove data</param>
        public void TorrentRemove(int[] ids, bool deleteData = false)
        {
            TorrentRemoveAsync(ids, deleteData).WaitAndUnwrapException();
        }

        #region Torrent Start
        /// <summary>
        /// Start torrents (API: torrent-start)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        public void TorrentStart(object[] ids)
        {
            TorrentStartAsync(ids).WaitAndUnwrapException();
        }

        /// <summary>
        /// Start recently active torrents (API: torrent-start)
        /// </summary>
        public void TorrentStart()
        {
            TorrentStartAsync().WaitAndUnwrapException();
        }
        #endregion

        #region Torrent Start Now

        /// <summary>
        /// Start now torrents (API: torrent-start-now)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        public void TorrentStartNow(object[] ids)
        {
            TorrentStartNowAsync(ids).WaitAndUnwrapException();
        }

        /// <summary>
        /// Start now recently active torrents (API: torrent-start-now)
        /// </summary>
        public void TorrentStartNow()
        {
            TorrentStartNowAsync().WaitAndUnwrapException();
        }
        #endregion

        #region Torrent Stop
        /// <summary>
        /// Stop torrents (API: torrent-stop)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        public void TorrentStop(object[] ids)
        {
            TorrentStopAsync(ids).WaitAndUnwrapException();
        }

        /// <summary>
        /// Stop recently active torrents (API: torrent-stop)
        /// </summary>
        public void TorrentStop()
        {
            TorrentStopAsync().WaitAndUnwrapException();
        }
        #endregion

        #region Torrent Verify
        /// <summary>
        /// Verify torrents (API: torrent-verify)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        public void TorrentVerify(object[] ids)
        {
            TorrentVerifyAsync(ids).WaitAndUnwrapException();
        }

        /// <summary>
        /// Verify recently active torrents (API: torrent-verify)
        /// </summary>
        public void TorrentVerify()
        {
            TorrentVerifyAsync().WaitAndUnwrapException();
        }
        #endregion

        /// <summary>
        /// Move torrents in queue on top (API: queue-move-top)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void TorrentQueueMoveTop(int[] ids)
        {
            TorrentQueueMoveTopAsync(ids).WaitAndUnwrapException();
        }

        /// <summary>
        /// Move up torrents in queue (API: queue-move-up)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentQueueMoveUp(int[] ids)
        {
            TorrentQueueMoveUpAsync(ids).WaitAndUnwrapException();
        }

        /// <summary>
        /// Move down torrents in queue (API: queue-move-down)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentQueueMoveDown(int[] ids)
        {
            TorrentQueueMoveDownAsync(ids).WaitAndUnwrapException();
        }

        /// <summary>
        /// Move torrents to bottom in queue  (API: queue-move-bottom)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentQueueMoveBottom(int[] ids)
        {
            TorrentQueueMoveBottomAsync(ids).WaitAndUnwrapException();
        }

        /// <summary>
        /// Set new location for torrents files (API: torrent-set-location)
        /// </summary>
        /// <param name="ids">Torrent ids</param>
        /// <param name="location">The new torrent location</param>
        /// <param name="move">Move from previous location</param>
        public void TorrentSetLocation(int[] ids, string location, bool move)
        {
            TorrentSetLocationAsync(ids, location, move).WaitAndUnwrapException();
        }

        /// <summary>
        /// Rename a file or directory in a torrent (API: torrent-rename-path)
        /// </summary>
        /// <param name="id">The torrent whose path will be renamed</param>
        /// <param name="path">The path to the file or folder that will be renamed</param>
        /// <param name="name">The file or folder's new name</param>
		public RenameTorrentInfo TorrentRenamePath(int id, string path, string name)
        {
            var task = TorrentRenamePathAsync(id, path, name);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        //method name not recognized
        ///// <summary>
        ///// Reannounce torrent (API: torrent-reannounce)
        ///// </summary>
        ///// <param name="ids"></param>
        //public void ReannounceTorrents(object[] ids)
        //{
        //    ReannounceTorrentsAsync(ids).WaitAndUnwrapException();
        //}

        #endregion

        #region System
        /// <summary>
        /// See if your incoming peer port is accessible from the outside world (API: port-test)
        /// </summary>
        /// <returns>Accessible state</returns>
        public bool PortTest()
        {
            var task = PortTestAsync();
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Update blocklist (API: blocklist-update)
        /// </summary>
        /// <returns>Blocklist size</returns>
        public int BlocklistUpdate()
        {
            var task = BlocklistUpdateAsync();
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Get free space is available in a client-specified folder.
        /// </summary>
        /// <param name="path">The directory to query</param>
        public long FreeSpace(string path)
        {
            var task = FreeSpaceAsync(path);
            task.WaitAndUnwrapException();
            return task.Result;
        }
        #endregion
    }
}
