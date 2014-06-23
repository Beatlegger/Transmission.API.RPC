using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Transmission.API.RPC.Entity;

namespace Transmission.API.RPC.Test
{
    [TestClass]
    public class MethodsTest
    {
        const string FILE_PATH = "./Data/ubuntu-10.04.4-server-amd64.iso.torrent";
        const string HOST = "http://192.168.1.50:9091/transmission/rpc";
        const string SESSION_ID = "";

        Client client = new Client(HOST, SESSION_ID);

        [TestMethod]
        public void AddTorrent()
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

            var torrentInfo = client.AddTorrent(torrent);
        }

        [TestMethod]
        public void GetTorrent()
        {
            var allTorrents = client.GetTorrents(TorrentFields.ALL_FIELDS);
            var oneTorrent = client.GetTorrents(TorrentFields.ALL_FIELDS, new int[] { 42 });
        }

        [TestMethod]
        public void RemoveTorrent()
        {
            client.RemoveTorrents(new int[] { 41 }); 
        }
    }
}
