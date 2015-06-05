using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections;
using System.Linq;
using Transmission.API.RPC.Entity;
using Transmission.API.RPC.Arguments;

namespace Transmission.API.RPC.Test
{
    [TestClass]
    public class MethodsTest
    {
        const string FILE_PATH = "./Data/ubuntu-10.04.4-server-amd64.iso.torrent";
        const string HOST = "http://192.168.1.2:9091/transmission/rpc";
        const string SESSION_ID = "";

        Client client = new Client(HOST, SESSION_ID);

        #region Torrent Test

        [TestMethod]
        public void AddGetSetAndRemoveTorrent()
        {
            if (!File.Exists(FILE_PATH))
                throw new Exception("Torrent file not found");

            var fstream = File.OpenRead(FILE_PATH);
            byte[] filebytes = new byte[fstream.Length];
            fstream.Read(filebytes, 0, Convert.ToInt32(fstream.Length));
            
            string encodedData = Convert.ToBase64String(filebytes, Base64FormattingOptions.InsertLineBreaks);

            var torrent = new NewTorrent
            {
                Metainfo = encodedData,
                Paused = true
            };

            var newTorrentInfo = client.AddTorrent(torrent);

			var allTorrents = client.GetTorrents(TorrentFields.ALL_FIELDS);

			Assert.IsNotNull(allTorrents);
			Assert.IsNotNull(allTorrents.Torrents);
			Assert.IsTrue(allTorrents.Torrents.Any(t => t.ID == newTorrentInfo.ID));

			var torrentInfo = allTorrents.Torrents.First(t => t.ID == newTorrentInfo.ID);

			TorrentSettings settings = new TorrentSettings()
			{
				IDs = new int[]
				{
					newTorrentInfo.ID
				},
				TrackerRemove = new int[] 
				{ 
					torrentInfo.Trackers[0].ID
				}
			};

			client.SetTorrents(settings);

			client.RemoveTorrents(new int[] { newTorrentInfo.ID });

			allTorrents = client.GetTorrents(TorrentFields.ALL_FIELDS);

			Assert.IsNotNull(allTorrents);
			Assert.IsNotNull(allTorrents.Torrents);
			Assert.IsFalse(allTorrents.Torrents.Any(t => t.ID == newTorrentInfo.ID));
        }

        #endregion

        #region Session Test

		[TestMethod]
		public void SessionGetTest()
		{
			var info = client.GetSessionInformation();
			Assert.IsNotNull(info);
			Assert.IsNotNull(info.Version);
		}
		
		[TestMethod]
        public void ChangeSessionTest()
        {
            //Get current session information
            var sessionInformation = client.GetSessionInformation();

			//Save old speed limit up
			var oldSpeedLimit = sessionInformation.SpeedLimitUp;

            //Set new speed limit
			sessionInformation.SpeedLimitUp = 200;

            //Set new session settings
			client.SetSessionSettings(sessionInformation);

            //Get new session information
            var newSessionInformation = client.GetSessionInformation();

			//Check new speed limit
			Assert.AreEqual(newSessionInformation.SpeedLimitUp, 200);
            
			//Restore speed limit
            newSessionInformation.SpeedLimitUp = oldSpeedLimit;

            //Set new session settinhs
            client.SetSessionSettings(newSessionInformation);
        }

        #endregion
    }
}
