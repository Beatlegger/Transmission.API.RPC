using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transmission.API.RPC.Entity;

namespace Transmission.API.RPC.Test
{
    [TestClass]
    public class SetSession_test
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
            var setSessionFull = new TransmissionSessionSet
            {
                AlternativeSpeedDown = 10000,
                AlternativeSpeedUp = 10000,
                AlternativeSpeedEnabled = false,
                AlternativeSpeedTimeBegin = 60,
                AlternativeSpeedTimeDay = 1,
                AlternativeSpeedTimeEnd = 1,
                AlternativeSpeedTimeEnabled = false,
                BlocklistURL = "http://www.example.com",
                BlocklistEnabled = false,
                CacheSizeMB = 100,
                DHTEnabled = true,
                DownloadDirectory = "/TorrentDownload",
                DownloadQueueEnabled = true,
                DownloadQueueSize = 50,
                Encryption = "required",
                IdleSeedingLimit = 0,
                IdleSeedingLimitEnabled = false,
                IncompleteDirectory = "/TorrentDownload",
                IncompleteDirectoryEnabled = false,
                LPDEnabled = false,
                PeerLimitGlobal = 100,
                PeerLimitPerTorrent = 10,
                PeerPort = 62555,
                PeerPortRandomOnStart = false,
                PexEnabled = true,
                PortForwardingEnabled = true,
                QueueStalledEnabled = false,
                QueueStalledMinutes = 10,
                RenamePartialFiles = true,
                ScriptTorrentDoneEnabled = false,
                ScriptTorrentDoneFilename = "none",
                SeedQueueEnabled = false,
                SeedQueueSize = 10,
                SeedRatioLimit = 10.5,
                SeedRatioLimited = false,
                SpeedLimitDown = 10,
                SpeedLimitDownEnabled = false,
                SpeedLimitUp = 10,
                SpeedLimitUpEnabled = false,
                StartAddedTorrents = true,
                TrashOriginalTorrentFiles = true,
                UtpEnabled = true,
                //Units = new TransmissionUnits
                //{
                //    ???
                //}

            };
            #endregion

            #region Some arguments
            var setSession = new TransmissionSessionSet
            {
                AlternativeSpeedDown = 100,
                AlternativeSpeedEnabled = true
            };
            #endregion

            var args = setSessionFull.ToArguments();
            var someArgs = setSession.ToArguments();
        }

        [TestMethod]
        public void SetSession()
        {
            var setSession = new TransmissionSessionSet
            {
                AlternativeSpeedDown = 777,
                AlternativeSpeedEnabled = true
            };

            client.SetSession(setSession);

        }
    }
}
