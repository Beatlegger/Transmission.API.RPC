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

        //files                       | array (see below)           | n/a
        //fileStats                   | array (see below)           | n/a

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

        //peers                       | array (see below)           | n/a

        [JsonProperty("peersConnected")]
        public int PeersConnected;

        //peersFrom                   | object (see below)          | n/a

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

        //priorities                  | array (see below)           | n/a

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

        //trackers                    | array (see below)           | n/a

        //trackerStats                | array (see below)           | n/a

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

        //wanted                      | array (see below)           | n/a

        //webseeds                    | array (see below)           | n/a

        [JsonProperty("webseedsSendingToUs")]
        public int WebseedsSendingToUs;
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
