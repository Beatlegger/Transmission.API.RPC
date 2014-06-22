using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transmission.API.RPC.Common;

namespace Transmission.API.RPC.Entity
{
    public class TransmissionTorrentsSet : TransmissionArgument
    {
        /// <summary>
        /// This torrent's bandwidth tr_priority_t
        /// </summary>
        [JsonProperty("bandwidthPriority")]
        public int? BandwidthPriority { get; set; }

        /// <summary>
        /// Maximum download speed (KBps)
        /// </summary>
        [JsonProperty("downloadLimit")]
        public int? DownloadLimit { get; set; }

        /// <summary>
        /// Download limit is honored
        /// </summary>
        [JsonProperty("downloadLimited")]
        public bool? DownloadLimited { get; set; }

        /// <summary>
        /// Session upload limits are honored
        /// </summary>
        [JsonProperty("honorsSessionLimits")]
        public bool? HonorsSessionLimits { get; set; }
        /// <summary>
        /// Torrent id array
        /// </summary>
        [JsonProperty("ids")]
        public int[] IDs { get; set; }

        /// <summary>
        /// New location of the torrent's content
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }

        /// <summary>
        /// Maximum number of peers
        /// </summary>
        [JsonProperty("peer-limit")]
        public int? PeerLimit { get; set; }

        /// <summary>
        /// Position of this torrent in its queue [0...n)
        /// </summary>
        [JsonProperty("queuePosition")]
        public int? QueuePosition { get; set; }

        /// <summary>
        /// Torrent-level number of minutes of seeding inactivity
        /// </summary>
        [JsonProperty("seedIdleLimit")]
        public int? SeedIdleLimit { get; set; }

        /// <summary>
        /// Which seeding inactivity to use
        /// </summary>
        [JsonProperty("seedIdleMode")]
        public int? SeedIdleMode { get; set; }

        /// <summary>
        /// Torrent-level seeding ratio
        /// </summary>
        [JsonProperty("seedRatioLimit")]
        public double? SeedRatioLimit { get; set; }

        /// <summary>
        /// Which ratio to use. 
        /// </summary>
        [JsonProperty("seedRatioMode")]
        public int? SeedRatioMode { get; set; }

        /// <summary>
        /// Maximum upload speed (KBps)
        /// </summary>
        [JsonProperty("uploadLimit")]
        public int? UploadLimit { get; set; }

        /// <summary>
        /// Upload limit is honored
        /// </summary>
        [JsonProperty("uploadLimited")]
        public bool? UploadLimited { get; set; }

        //"files-wanted"        | array      indices of file(s) to download
        //public [] FilesWanted;

        //"files-unwanted"      | array      indices of file(s) to not download
        //public [] FilesUnwanted;

        //"trackerAdd"          | array      strings of announce URLs to add
        //public [] TrackerAdd;

        //"trackerRemove"       | array      ids of trackers to remove
        //public [] trackerRemove;

        //"trackerReplace"      | array      pairs of <trackerId/new announce URLs>
        //public [] trackerReplace;

        //"priority-high"       | array      indices of high-priority file(s)
        //public [] PriorityHigh;

        //"priority-low"        | array      indices of low-priority file(s)
        //public [] PriorityLow;

        //"priority-normal"     | array      indices of normal-priority file(s)
        //public [] PriorityNormal;

    }
}
