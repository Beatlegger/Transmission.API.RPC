using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transsmission.API.RPC.Entity
{
    public class TransmissionSessionStat
    {
        [JsonProperty("activeTorrentCount")]
        public int ActiveTorrentCount;

        [JsonProperty("downloadSpeed")]
        public int downloadSpeed;

        [JsonProperty("pausedTorrentCount")]
        public int pausedTorrentCount;

        [JsonProperty("torrentCount")]
        public int torrentCount;

        [JsonProperty("uploadSpeed")]
        public int uploadSpeed;
   
        [JsonProperty("cumulative-stats")]
        public TransmissionCommonStats CumulativeStats;
 
        [JsonProperty("current-stats")]
        public TransmissionCommonStats CurrentStats;
    }

    public class TransmissionCommonStats
    {
        [JsonProperty("uploadedBytes")]
        public double uploadedBytes;
        
        [JsonProperty("downloadedBytes")]
        public double DownloadedBytes;

        [JsonProperty("filesAdded")]
        public int FilesAdded;

        [JsonProperty("SessionCount")]
        public int SessionCount;

        [JsonProperty("SecondsActive")]
        public int SecondsActive;
    }
}
