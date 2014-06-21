using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transmission.API.RPC.Entity;
using System.IO;

namespace Transmission.API.RPC.Test
{
    [TestClass]
    public class AddTorrent
    {
        const string SESSION_ID = "eyCd0F1Yxc1ljG4GbROe3542HH6PneD3NjcqmXtnKT6E5Y6b";
        const string HOST = "http://192.168.1.5:9091/transmission/rpc";
        const string TEST_TORRENT_FILE = ".\\Data\\ubuntu-10.04.4-server-amd64.iso.torrent";

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
            var newFullTorrent = new TransmissionNewTorrent
            {
                BandwidthPriority = 1,
                Cookies = "Cookies Value",
                DownloadDirectory = "Download Directory Value",
                Filename = "File Name",
                FilesUnwanted = new string[]
                {
                    "Files Unwanted Value 1",
                    "Files Unwanted Value 2"
                },
                FilesWanted = new string[]
                {
                    "Files Unwanted Value 1",
                    "Files Unwanted Value 2"
                },
                Metainfo = "Metainfo Value",
                Paused = true,
                PeerLimit = 10,
                PriorityHigh = new string[]
                {
                    "Priority High Value 1",
                    "Priority High Value 2"
                },
                PriorityLow = new string[]
                {
                    "Priority Low Value 1",
                    "Priority Low Value 2"
                },
                PriorityNormal = new string[]
                {
                    "Priority Normal Value 1",
                    "Priority Normal Value 2"
                }
            };
            #endregion

            #region Some arguments
            var newTorrent = new TransmissionNewTorrent
            {
                DownloadDirectory = "Download Directory Value",
                Metainfo = "Metainfo Value",
                Paused = true,
            };
            #endregion

            var fullArguments = newFullTorrent.ToArguments();
            var someArguments = newTorrent.ToArguments();         
        }

        [TestMethod]
        public void AddTorrent_File()
        {
            var fstream = File.Open(TEST_TORRENT_FILE, FileMode.Open);
            Byte[] inArray = new Byte[(int)fstream.Length];
            fstream.Read(inArray, 0, (int)fstream.Length);

            var metainfo = Convert.ToBase64String(inArray, 0, inArray.Length);

            var newTorrent = new TransmissionNewTorrent
            {
                Metainfo = metainfo,
                Paused = true,
                DownloadDirectory = "/TorrentDownload_Move",
            };

            var addResult = client.AddTorrent(newTorrent);

            var torrentInfo = client.GetTorrents(TransmisiionTorrentFields.ALL_FIELDS, new int[] { addResult.ID });
        }

        [TestMethod]
        public void AddTorrent_URL()
        {
            var newTorrent = new TransmissionNewTorrent
            {
                Filename = "http://releases.ubuntu.com/10.04.4/ubuntu-10.04.4-server-i386.iso.torrent",
                Paused = true,
                DownloadDirectory = "/TorrentDownload_Move",
            };

            var addResult = client.AddTorrent(newTorrent);

            var torrentInfo = client.GetTorrents(TransmisiionTorrentFields.ALL_FIELDS, new int[] { addResult.ID });
        }
    }
}
