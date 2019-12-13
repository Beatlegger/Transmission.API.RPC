using Transmission.API.RPC.Arguments;
using Transmission.API.RPC.Entity;

namespace Transmission.API.RPC
{
    public interface ITransmissionClient
    {
        int CurrentTag { get; }
        string SessionID { get; }
        string Url { get; }

        /// <summary>
        /// Update blocklist (API: blocklist-update)
        /// </summary>
        /// <returns>Blocklist size</returns>
        int BlocklistUpdate();

        /// <summary>
        /// Close current session (API: session-close)
        /// </summary>
        void CloseSession();

        /// <summary>
        /// Get free space is available in a client-specified folder.
        /// </summary>
        /// <param name="path">The directory to query</param>
        long FreeSpace(string path);

        /// <summary>
        /// Get information of current session (API: session-get)
        /// </summary>
        /// <returns>Session information</returns>
        SessionInfo GetSessionInformation();

        /// <summary>
        /// Get session stat
        /// </summary>
        /// <returns>Session stat</returns>
        Statistic GetSessionStatistic();

        /// <summary>
        /// See if your incoming peer port is accessible from the outside world (API: port-test)
        /// </summary>
        /// <returns>Accessible state</returns>
        bool PortTest();

        /// <summary>
        /// Set information to current session (API: session-set)
        /// </summary>
        /// <param name="settings">New session settings</param>
        void SetSessionSettings(SessionSettings settings);

        /// <summary>
        /// Add torrent (API: torrent-add)
        /// </summary>
        /// <returns>Torrent info (ID, Name and HashString)</returns>
        NewTorrentInfo TorrentAdd(NewTorrent torrent);

        /// <summary>
        /// Get fields of torrents from ids (API: torrent-get)
        /// </summary>
        /// <param name="fields">Fields of torrents</param>
        /// <param name="ids">IDs of torrents (null or empty for get all torrents)</param>
        /// <returns>Torrents info</returns>
        TransmissionTorrents TorrentGet(string[] fields, params int[] ids);

        /// <summary>
        /// Move torrents to bottom in queue  (API: queue-move-bottom)
        /// </summary>
        /// <param name="ids"></param>
        void TorrentQueueMoveBottom(int[] ids);

        /// <summary>
        /// Move down torrents in queue (API: queue-move-down)
        /// </summary>
        /// <param name="ids"></param>
        void TorrentQueueMoveDown(int[] ids);

        /// <summary>
        /// Move torrents in queue on top (API: queue-move-top)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        void TorrentQueueMoveTop(int[] ids);

        /// <summary>
        /// Move up torrents in queue (API: queue-move-up)
        /// </summary>
        /// <param name="ids"></param>
        void TorrentQueueMoveUp(int[] ids);

        /// <summary>
        /// Remove torrents (API: torrent-remove)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        /// <param name="deleteLocalData">Remove local data</param>
        void TorrentRemove(int[] ids, bool deleteData = false);

        /// <summary>
        /// Rename a file or directory in a torrent (API: torrent-rename-path)
        /// </summary>
        /// <param name="ids">The torrent whose path will be renamed</param>
        /// <param name="path">The path to the file or folder that will be renamed</param>
        /// <param name="name">The file or folder's new name</param>
        RenameTorrentInfo TorrentRenamePath(int id, string path, string name);

        /// <summary>
        /// Set torrent params (API: torrent-set)
        /// </summary>
        /// <param name="torrentSet">New torrent params</param>
        void TorrentSet(TorrentSettings settings);

        /// <summary>
        /// Set new location for torrents files (API: torrent-set-location)
        /// </summary>
        /// <param name="ids">Torrent ids</param>
        /// <param name="location">The new torrent location</param>
        /// <param name="move">Move from previous location</param>
        void TorrentSetLocation(int[] ids, string location, bool move);

        /// <summary>
        /// Start recently active torrents (API: torrent-start)
        /// </summary>
        void TorrentStart();

        /// <summary>
        /// Start torrents (API: torrent-start)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        void TorrentStart(object[] ids);

        /// <summary>
        /// Start now recently active torrents (API: torrent-start-now)
        /// </summary>
        void TorrentStartNow();

        /// <summary>
        /// Start now torrents (API: torrent-start-now)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        void TorrentStartNow(object[] ids);

        /// <summary>
        /// Stop recently active torrents (API: torrent-stop)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        void TorrentStop();

        /// <summary>
        /// Stop torrents (API: torrent-stop)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        void TorrentStop(object[] ids);

        /// <summary>
        /// Verify recently active torrents (API: torrent-verify)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        void TorrentVerify();

        /// <summary>
        /// Verify torrents (API: torrent-verify)
        /// </summary>
        /// <param name="ids">A list of torrent id numbers, sha1 hash strings, or both</param>
        void TorrentVerify(object[] ids);
    }
}