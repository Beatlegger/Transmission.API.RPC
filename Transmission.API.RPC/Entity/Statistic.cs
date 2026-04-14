using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.API.RPC.Entity
{
    /// <summary>
    /// Statistic
    /// </summary>
    public class Statistic
    {
        /// <summary>
        /// Active torrent count
        /// </summary>
        [JsonPropertyName("activeTorrentCount")]
        public int ActiveTorrentCount { get; set; }

        /// <summary>
        /// Download speed
        /// </summary>
        [JsonPropertyName("downloadSpeed")]
        public int downloadSpeed{ get; set; }

        /// <summary>
        /// Paused torrent count
        /// </summary>
        [JsonPropertyName("pausedTorrentCount")]
        public int pausedTorrentCount{ get; set; }

        /// <summary>
        /// Torrent count
        /// </summary>
        [JsonPropertyName("torrentCount")]
        public int torrentCount{ get; set; }

        /// <summary>
        /// Upload speed
        /// </summary>
        [JsonPropertyName("uploadSpeed")]
        public int uploadSpeed{ get; set; }
   
        /// <summary>
        /// Cumulative stats
        /// </summary>
        [JsonPropertyName("cumulative-stats")]
        public CommonStatistic CumulativeStats { get; set; }
 
        /// <summary>
        /// Current stats
        /// </summary>
        [JsonPropertyName("current-stats")]
        public CommonStatistic CurrentStats { get; set; }
    }

    /// <summary>
    /// Common statistic
    /// </summary>
    public class CommonStatistic
    {
        /// <summary>
        /// Uploaded bytes
        /// </summary>
        [JsonPropertyName("uploadedBytes")]
        public double uploadedBytes{ get; set; }
        
        /// <summary>
        /// Downloaded bytes
        /// </summary>
        [JsonPropertyName("downloadedBytes")]
        public double DownloadedBytes{ get; set; }

        /// <summary>
        /// Files added
        /// </summary>
        [JsonPropertyName("filesAdded")]
        public int FilesAdded{ get; set; }

        /// <summary>
        /// Session count
        /// </summary>
        [JsonPropertyName("SessionCount")]
        public int SessionCount{ get; set; }

        /// <summary>
        /// Seconds active
        /// </summary>
        [JsonPropertyName("SecondsActive")]
        public int SecondsActive{ get; set; }
    }
}
