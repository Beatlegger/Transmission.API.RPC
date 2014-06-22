using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transmission.API.RPC.Entity;

namespace Transmission.API.RPC.Test
{
    [TestClass]
    public class SetTorrent
    {
        const string SESSION_ID = "eyCd0F1Yxc1ljG4GbROe3542HH6PneD3NjcqmXtnKT6E5Y6b";
        const string HOST = "http://192.168.1.5:9091/transmission/rpc";

        public Client client = new Client();

        [TestInitialize]
        public void Initialize()
        {
            client.Host = HOST;
            client.SessionID = SESSION_ID;
        }

        [TestMethod]
        public void ConvertAttributes()
        {
            #region Full arguments
            var setTorrentsFull = new TransmissionTorrentsSet
            {
                BandwidthPriority = 1,
                DownloadLimit = 1000,
                DownloadLimited = false,
                HonorsSessionLimits = false,
                IDs = new int[]
               {
                   1,
                   2
               },
                Location = "/",
                PeerLimit = 1,
                QueuePosition = 1,
                SeedIdleLimit = 1,
                SeedIdleMode = 1,
                SeedRatioLimit = 1,
                SeedRatioMode = 1,
                UploadLimit = 1,
                UploadLimited = true
            };
            #endregion

            #region Some arguments
            var setTorrents = new TransmissionTorrentsSet
            {
                BandwidthPriority = 1,
                IDs = new int[]
               {
                   1,
                   2
               },
            };
            #endregion

            var args = setTorrentsFull.ToArguments();
            var someArgs = setTorrents.ToArguments();
        }

        [TestMethod]
        public void Set()
        {
            var setTorrents = new TransmissionTorrentsSet
            {
                DownloadLimit = 1000,
                DownloadLimited = true,
                IDs = new int[]
                {
                    1, 
                    2
                },
                Location = "/TorrentDownload_Move",
                PeerLimit = 10
            };

            client.SetTorrents(setTorrents);
        }
    }
}
