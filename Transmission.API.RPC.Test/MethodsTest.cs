using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Transmission.API.RPC.Entity;
using Transmission.API.RPC.Arguments;

namespace Transmission.API.RPC.Test
{
    /// <summary>
    /// Shared fixture that ensures a test torrent exists for the duration of the test class.
    /// </summary>
    public class TransmissionFixture : IAsyncLifetime
    {
        const string FILE_PATH = "./Data/ubuntu-10.04.4-server-amd64.iso.torrent";
        const string HOST = "http://localhost:9091/transmission/rpc";

        public Client Client { get; }
        public int TorrentId { get; private set; }
        public string TorrentHashString { get; private set; }

        public TransmissionFixture()
        {
            // Use Client's default HttpClient (zero pooled-connection lifetime).
            // Transmission closes keep-alive connections aggressively — especially
            // after session-close — so a persistent pool would hand out stale
            // sockets that fail with HttpIOException: ResponseEnded, or return
            // empty query results tied to an invalidated server-side session.
            Client = new Client(HOST);
        }

        public async Task InitializeAsync()
        {
            if (!File.Exists(FILE_PATH))
                throw new FileNotFoundException("Torrent file not found", FILE_PATH);

            using var fstream = File.OpenRead(FILE_PATH);
            byte[] filebytes = new byte[fstream.Length];
            fstream.ReadExactly(filebytes, 0, Convert.ToInt32(fstream.Length));
            string encodedData = Convert.ToBase64String(filebytes);

            var torrent = new NewTorrent
            {
                Metainfo = encodedData,
                Paused = true
            };

            var info = await Client.TorrentAddAsync(torrent);
            TorrentId = info.ID;
            TorrentHashString = info.HashString;
        }

        public async Task DisposeAsync()
        {
            try
            {
                await Client.TorrentRemoveAsync(new int[] { TorrentId }, deleteData: true);
            }
            catch
            {
                // Best-effort cleanup
            }
        }
    }

    [CollectionDefinition("Integration", DisableParallelization = true)]
    public class IntegrationCollection : ICollectionFixture<TransmissionFixture> { }

    /// <summary>
    /// Integration tests — require a running Transmission daemon at localhost:9091.
    /// </summary>
    [Collection("Integration")]
    public class MethodsTest
    {
        private readonly TransmissionFixture _fixture;
        private Client Client => _fixture.Client;

        public MethodsTest(TransmissionFixture fixture)
        {
            _fixture = fixture;
        }

        #region Torrent Tests

        [Fact]
        public void AddTorrent_Test()
        {
            Assert.True(_fixture.TorrentId != 0);
            Assert.NotNull(_fixture.TorrentHashString);
        }

        [Fact]
        public async Task GetTorrentInfo_Test()
        {
            var torrentsInfo = await Client.TorrentGetAsync(TorrentFields.ALL_FIELDS);

            Assert.NotNull(torrentsInfo);
            Assert.NotNull(torrentsInfo.Torrents);
            Assert.NotEmpty(torrentsInfo.Torrents);
        }

        [Fact]
        public async Task GetTorrentById_Test()
        {
            var torrentsInfo = await Client.TorrentGetAsync(TorrentFields.ALL_FIELDS, _fixture.TorrentId);

            Assert.NotNull(torrentsInfo);
            Assert.NotEmpty(torrentsInfo.Torrents);
            Assert.Equal(_fixture.TorrentId, torrentsInfo.Torrents[0].ID);
        }

        [Fact]
        public async Task SetTorrentSettings_Test()
        {
            var torrentsInfo = await Client.TorrentGetAsync(TorrentFields.ALL_FIELDS, _fixture.TorrentId);
            var torrentInfo = torrentsInfo.Torrents.FirstOrDefault();
            Assert.NotNull(torrentInfo);

            var trackerInfo = torrentInfo.Trackers.FirstOrDefault();
            Assert.NotNull(trackerInfo);
            var trackerCount = torrentInfo.Trackers.Length;

            TorrentSettings settings = new TorrentSettings()
            {
                IDs = new object[] { torrentInfo.HashString },
                TrackerRemove = new int[] { trackerInfo.ID }
            };

            await Client.TorrentSetAsync(settings);

            torrentsInfo = await Client.TorrentGetAsync(TorrentFields.ALL_FIELDS, torrentInfo.ID);
            torrentInfo = torrentsInfo.Torrents.FirstOrDefault();

            Assert.NotEqual(trackerCount, torrentInfo.Trackers.Length);
        }

        [Fact]
        public async Task RenamePathTorrent_Test()
        {
            var torrentsInfo = await Client.TorrentGetAsync(TorrentFields.ALL_FIELDS, _fixture.TorrentId);
            var torrentInfo = torrentsInfo.Torrents.FirstOrDefault();
            Assert.NotNull(torrentInfo);

            var originalName = torrentInfo.Files[0].Name;
            var newName = "test_" + originalName;

            var result = await Client.TorrentRenamePathAsync(torrentInfo.ID, originalName, newName);
            Assert.NotNull(result);
            Assert.True(result.ID != 0);

            // Restore original name
            await Client.TorrentRenamePathAsync(torrentInfo.ID, newName, originalName);
        }

        [Fact]
        public async Task RemoveTorrent_Test()
        {
            // Add a distinct torrent via a random-hash magnet link so we don't
            // collide with the fixture torrent (Transmission would otherwise
            // return torrent-duplicate with the fixture's ID, and removing it
            // would corrupt state for the rest of the test class).
            var randomHash = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(20));
            var torrent = new NewTorrent
            {
                Filename = $"magnet:?xt=urn:btih:{randomHash}&dn=removal-test",
                Paused = true
            };

            var addedTorrent = await Client.TorrentAddAsync(torrent);
            Assert.NotNull(addedTorrent);
            Assert.NotEqual(_fixture.TorrentId, addedTorrent.ID);

            await Client.TorrentRemoveAsync(new int[] { addedTorrent.ID });

            var torrentsInfo = await Client.TorrentGetAsync(TorrentFields.ALL_FIELDS);
            Assert.DoesNotContain(torrentsInfo.Torrents, t => t.ID == addedTorrent.ID);
        }

        #endregion

        #region Session Tests

        [Fact]
        public async Task SessionGet_Test()
        {
            var info = await Client.GetSessionInformationAsync();
            Assert.NotNull(info);
            Assert.NotNull(info.Version);
        }

        [Fact]
        public async Task ChangeSession_Test()
        {
            var sessionInformation = await Client.GetSessionInformationAsync();
            var oldSpeedLimit = sessionInformation.SpeedLimitUp;

            await Client.SetSessionSettingsAsync(new SessionSettings() { SpeedLimitUp = 100 });

            var newSessionInformation = await Client.GetSessionInformationAsync();
            Assert.Equal(100, newSessionInformation.SpeedLimitUp);

            // Restore original value
            await Client.SetSessionSettingsAsync(new SessionSettings() { SpeedLimitUp = oldSpeedLimit });
        }

        [Fact]
        public async Task GetSessionStatistic_Test()
        {
            var stats = await Client.GetSessionStatisticAsync();
            Assert.NotNull(stats);
            Assert.True(stats.ActiveTorrentCount >= 0);
            Assert.True(stats.downloadSpeed >= 0);
            Assert.True(stats.uploadSpeed >= 0);
        }

        [Fact]
        public async Task BlocklistUpdate_Test()
        {
            const string validBlocklistUrl =
                "http://list.iblocklist.com/?list=bt_level1&fileformat=p2p&archiveformat=gz";

            var sessionInfo = await Client.GetSessionInformationAsync();
            var originalUrl = sessionInfo.BlocklistURL;
            var originalEnabled = sessionInfo.BlocklistEnabled;

            try
            {
                await Client.SetSessionSettingsAsync(new SessionSettings
                {
                    BlocklistURL = validBlocklistUrl,
                    BlocklistEnabled = true
                });

                var result = await Client.BlocklistUpdateAsync();
                Assert.True(result >= 0);
            }
            finally
            {
                await Client.SetSessionSettingsAsync(new SessionSettings
                {
                    BlocklistURL = originalUrl,
                    BlocklistEnabled = originalEnabled
                });
            }
        }

        [Fact]
        public async Task FreeSpace_Test()
        {
            var result = await Client.FreeSpaceAsync("/");
            Assert.True(result >= 0);
        }

        [Fact]
        public async Task PortTest_Test()
        {
            var result = await Client.PortTestAsync();
            Assert.IsType<bool>(result);
        }

        // NOTE: session-close is intentionally not covered here. Empirically on
        // Transmission 4.1.1 the RPC does not shut down the daemon, but it DOES
        // invalidate server-side session state in a way that makes subsequent
        // torrent-get-by-id queries in the same shared fixture sporadically
        // return empty results. Running it in this collection corrupts every
        // test that executes afterwards. A unit-test project (with a mocked
        // HttpMessageHandler) is the right home for verifying that
        // CloseSessionAsync sends the session-close RPC.

        #endregion

        #region Queue Tests

        [Fact]
        public async Task TorrentQueueMoveBottom_Test()
        {
            await Client.TorrentQueueMoveBottomAsync(new int[] { _fixture.TorrentId });
        }

        [Fact]
        public async Task TorrentQueueMoveDown_Test()
        {
            await Client.TorrentQueueMoveDownAsync(new int[] { _fixture.TorrentId });
        }

        [Fact]
        public async Task TorrentQueueMoveTop_Test()
        {
            await Client.TorrentQueueMoveTopAsync(new int[] { _fixture.TorrentId });
        }

        [Fact]
        public async Task TorrentQueueMoveUp_Test()
        {
            await Client.TorrentQueueMoveUpAsync(new int[] { _fixture.TorrentId });
        }

        #endregion

        #region Torrent Management Tests

        [Fact]
        public async Task TorrentSetLocation_Test()
        {
            // Transmission requires an absolute path; reuse current download-dir
            // with move=false so nothing is actually relocated.
            var sessionInfo = await Client.GetSessionInformationAsync();
            await Client.TorrentSetLocationAsync(
                new int[] { _fixture.TorrentId },
                sessionInfo.DownloadDirectory,
                false);
        }

        [Fact]
        public async Task TorrentStart_Test()
        {
            await Client.TorrentStartAsync(new object[] { _fixture.TorrentId });
        }

        [Fact]
        public async Task TorrentStartNow_Test()
        {
            await Client.TorrentStartNowAsync(new object[] { _fixture.TorrentId });
        }

        [Fact]
        public async Task TorrentStop_Test()
        {
            await Client.TorrentStopAsync(new object[] { _fixture.TorrentId });
        }

        [Fact]
        public async Task TorrentVerify_Test()
        {
            await Client.TorrentVerifyAsync(new object[] { _fixture.TorrentId });
        }

        [Fact]
        public async Task TorrentStartAll_Test()
        {
            await Client.TorrentStartAsync();
        }

        [Fact]
        public async Task TorrentStartNowAll_Test()
        {
            await Client.TorrentStartNowAsync();
        }

        [Fact]
        public async Task TorrentStopAll_Test()
        {
            await Client.TorrentStopAsync();
        }

        [Fact]
        public async Task TorrentVerifyAll_Test()
        {
            await Client.TorrentVerifyAsync();
        }

        #endregion
    }
}
