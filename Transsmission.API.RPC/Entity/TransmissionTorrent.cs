using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transsmission.API.RPC.Entity
{
    /// <summary>
    /// Torrent information
    /// </summary>
    public class TransmissionTorrent
    {
        /// <summary>
        /// The torrent's unique Id.
        /// </summary>
        [JsonProperty("id")]
        public int ID;

        [JsonProperty("addedDate")]
        public int AddedDate;

        [JsonProperty("bandwidthPriority")]
        public int BandwidthPriority;

        [JsonProperty("comment")]
        public string Comment;

        [JsonProperty("corruptEver")]
        public int CorruptEver;

        [JsonProperty("creator")]
        public string Creator;

        [JsonProperty("dateCreated")]
        public int DateCreated;

        [JsonProperty("desiredAvailable")]
        public int DesiredAvailable;

        [JsonProperty("doneDate")]
        public int DoneDate;

        [JsonProperty("downloadDir")]
        public string DownloadDir;

        [JsonProperty("downloadedEver")]
        public string DownloadedEver;

        [JsonProperty("downloadLimit")]
        public string DownloadLimit;

        [JsonProperty("downloadLimited")]
        public string DownloadLimited;

        [JsonProperty("error")]
        public int Error;

        [JsonProperty("ErrorString")]
        public string ErrorString;

        [JsonProperty("eta")]
        public int ETA;

        [JsonProperty("etaIdle")]
        public int ETAIdle;

        [JsonProperty("files")]
        public TransmissionTorrentFiles[] Files;

        [JsonProperty("fileStats")]
        public TransmissionTorrentFileStats[] FileStats;

        [JsonProperty("hashString")]
        public string HashString;

        [JsonProperty("haveUnchecked")]
        public int HaveUnchecked;

        [JsonProperty("haveValid")]
        public long HaveValid;

        [JsonProperty("honorsSessionLimits")]
        public bool HonorsSessionLimits;

        [JsonProperty("isFinished")]
        public bool IsFinished;

        [JsonProperty("isPrivate")]
        public bool IsPrivate;

        [JsonProperty("isStalled")]
        public bool IsStalled;

        [JsonProperty("leftUntilDone")]
        public int LeftUntilDone;

        [JsonProperty("MagnetLink")]
        public string MagnetLink;

        [JsonProperty("manualAnnounceTime")]
        public int ManualAnnounceTime;

        [JsonProperty("maxConnectedPeers")]
        public int MaxConnectedPeers;

        [JsonProperty("metadataPercentComplete")]
        public double MetadataPercentComplete;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("peer-limit")]
        public int PeerLimit;

        [JsonProperty("peers")]
        public TransmissionTorrentPeers[] Peers;

        [JsonProperty("peersConnected")]
        public int PeersConnected;

        [JsonProperty("peersFrom")]
        public TransmissionTorrentPeersFrom PeersFrom;

        [JsonProperty("peersSendingToUs")]
        public int PeersSendingToUs;

        [JsonProperty("percentDone")]
        public double PercentDone;

        [JsonProperty("pieces")]
        public string Pieces;

        [JsonProperty("pieceCount")]
        public int PieceCount;

        [JsonProperty("PieceSize")]
        public int PieceSize;

        [JsonProperty("priorities")]
        public int[] Priorities;

        [JsonProperty("queuePosition")]
        public int QueuePosition;

        [JsonProperty("rateDownload")]
        public int RateDownload;

        [JsonProperty("rateUpload")]
        public int RateUpload;

        [JsonProperty("recheckProgress")]
        public double RecheckProgress;

        [JsonProperty("secondsDownloading")]
        public int SecondsDownloading;

        [JsonProperty("secondsSeeding")]
        public int SecondsSeeding;

        [JsonProperty("seedIdleLimit")]
        public int SeedIdleLimit;

        [JsonProperty("SeedIdleMode")]
        public int SeedIdleMode;

        [JsonProperty("seedRatioLimit")]
        public double SeedRatioLimit;

        [JsonProperty("SeedRatioMode")]
        public int SeedRatioMode;

        [JsonProperty("SizeWhenDone")]
        public long SizeWhenDone;

        [JsonProperty("startDate")]
        public int StartDate;

        [JsonProperty("Status")]
        public int Status;

        [JsonProperty("trackers")]
        public TransmissionTorrentTrackers[] Trackers;

        [JsonProperty("trackerStats")]
        TransmissionTorrentTrackerStats[] TrackerStats;

        [JsonProperty("totalSize")]
        public long TotalSize;

        [JsonProperty("torrentFile")]
        public string TorrentFile;

        [JsonProperty("uploadedEver")]
        public long UploadedEver;

        [JsonProperty("uploadLimit")]
        public int UploadLimit;

        [JsonProperty("uploadLimited")]
        public bool UploadLimited;

        [JsonProperty("uploadRatio")]
        public double uploadRatio;

        [JsonProperty("wanted")]
        public bool[] Wanted;

        [JsonProperty("webseeds")]
        public string[] Webseeds;

        [JsonProperty("webseedsSendingToUs")]
        public int WebseedsSendingToUs;
    }

    public class TransmissionTorrentFiles
    {
        [JsonProperty("bytesCompleted")]
        public double BytesCompleted;

        [JsonProperty("length")]
        public double Length;

        [JsonProperty("name")]
        public string Name;
    }

    public class TransmissionTorrentFileStats
    {
        [JsonProperty("bytesCompleted")]
        public double BytesCompleted;

        [JsonProperty("wanted")]
        public bool Wanted;

        [JsonProperty("priority")]
        public int Priority;
    }

    public class TransmissionTorrentPeers
    {
        [JsonProperty("address")]
        public string Address;

        [JsonProperty("clientName")]
        public string ClientName;

        [JsonProperty("clientIsChoked")]
        public bool ClientIsChoked;

        [JsonProperty("clientIsInterested")]
        public bool ClientIsInterested;

        [JsonProperty("flagStr")]
        public string FlagStr;

        [JsonProperty("isDownloadingFrom")]
        public bool IsDownloadingFrom;

        [JsonProperty("isEncrypted")]
        public bool IsEncrypted;

        [JsonProperty("isUploadingTo")]
        public bool IsUploadingTo;

        [JsonProperty("isUTP")]
        public bool IsUTP;

        [JsonProperty("peerIsChoked")]
        public bool PeerIsChoked;

        [JsonProperty("peerIsInterested")]
        public bool PeerIsInterested;

        [JsonProperty("port")]
        public int Port;

        [JsonProperty("progress")]
        public double Progress;

        [JsonProperty("rateToClient")]
        public int RateToClient;

        [JsonProperty("rateToPeer")]
        public int RateToPeer;
    }

    public class TransmissionTorrentPeersFrom
    {
        [JsonProperty("fromDht")]
        public int FromDHT;

        [JsonProperty("fromIncoming")]
        public int FromIncoming;

        [JsonProperty("fromLpd")]
        public int FromLPD;

        [JsonProperty("fromLtep")]
        public int FromLTEP;

        [JsonProperty("fromPex")]
        public int FromPEX;

        [JsonProperty("fromTracker")]
        public int FromTracker;
    }

    public class TransmissionTorrentTrackers
    {

        [JsonProperty("announce")]
        public string announce;

        [JsonProperty("id")]
        public int ID;

        [JsonProperty("scrape")]
        public string Scrape;

        [JsonProperty("tier")]
        public int Tier;
    }

    public class TransmissionTorrentTrackerStats
    {

        [JsonProperty("announce")]
        public string announce;

        [JsonProperty("announceState")]
        public int AnnounceState;

        [JsonProperty("downloadCount")]
        public int DownloadCount;

        [JsonProperty("hasAnnounced")]
        public bool HasAnnounced;

        [JsonProperty("hasScraped")]
        public bool HasScraped;

        [JsonProperty("host")]
        public string Host;

        [JsonProperty("isBackup")]
        public bool IsBackup;

        [JsonProperty("lastAnnouncePeerCount")]
        public int LastAnnouncePeerCount;

        [JsonProperty("id")]
        public int ID;

        [JsonProperty("lastAnnounceResult")]
        public string LastAnnounceResult;

        [JsonProperty("lastAnnounceSucceeded")]
        public bool LastAnnounceSucceeded;

        [JsonProperty("lastAnnounceStartTime")]
        public int LastAnnounceStartTime;

        [JsonProperty("lastScrapeResult")]
        public string LastScrapeResult;

        [JsonProperty("lastAnnounceTimedOut")]
        public bool LastAnnounceTimedOut;

        [JsonProperty("lastAnnounceTime")]
        public int LastAnnounceTime;

        [JsonProperty("lastScrapeSucceeded")]
        public bool LastScrapeSucceeded;

        [JsonProperty("lastScrapeStartTime")]
        public int LastScrapeStartTime;

        [JsonProperty("lastScrapeTimedOut")]
        public bool LastScrapeTimedOut;

        [JsonProperty("lastScrapeTime")]
        public int LastScrapeTime;

        [JsonProperty("scrape")]
        public string Scrape;

        [JsonProperty("tier")]
        public int Tier;

        [JsonProperty("leecherCount")]
        public int LeecherCount;

        [JsonProperty("nextAnnounceTime")]
        public int NextAnnounceTime;

        [JsonProperty("nextScrapeTime")]
        public int NextScrapeTime;

        [JsonProperty("scrapeState")]
        public int ScrapeState;

        [JsonProperty("seederCount")]
        public int SeederCount;
    }

    //TODO: Separate "remove" and "active" torrents in "torrentsGet"
    /// <summary>
    /// Contains arrays of torrents and removed torrents
    /// </summary>
    public class TransmissionTorrents
    {
        /// <summary>
        /// Array of torrents
        /// </summary>
        [JsonProperty("torrents")]
        public TransmissionTorrent[] Torrents;

        /// <summary>
        /// Array of torrent-id numbers of recently-removed torrents
        /// </summary>
        [JsonProperty("removed")]
        public TransmissionTorrent[] Removed;
    }
}
