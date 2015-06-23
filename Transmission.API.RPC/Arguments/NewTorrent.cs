using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transmission.API.RPC.Common;

namespace Transmission.API.RPC.Entity
{
    /// <summary>
    /// Information about the torrent file, that will be added
    /// </summary>
    public class NewTorrent : ArgumentsBase
    {
        /// <summary>
        /// Pointer to a string of one or more cookies.
        /// </summary>
        [JsonProperty("cookies")]
        public string Cookies{ get; set; }

        /// <summary>
        /// Path to download the torrent to
        /// </summary>
        [JsonProperty("download-dir")]
        public string DownloadDirectory{ get; set; }

        /// <summary>
		/// filename (relative to the server) or URL of the .torrent file (Priority than the metadata)
        /// </summary>
        [JsonProperty("filename")]
        public string Filename{ get; set; }

        /// <summary>
        /// base64-encoded .torrent content
        /// </summary>
        [JsonProperty("metainfo")]
        public string Metainfo{ get; set; }

        /// <summary>
        /// if true, don't start the torrent
        /// </summary>
        [JsonProperty("paused")]
        public bool Paused{ get; set; }

        /// <summary>
        /// maximum number of peers
        /// </summary>
        [JsonProperty("peer-limit")]
        public int? PeerLimit{ get; set; }

        /// <summary>
        /// Torrent's bandwidth priority
        /// </summary>
        [JsonProperty("bandwidthPriority")]
        public int? BandwidthPriority{ get; set; }

        /// <summary>
        /// Indices of file(s) to download
        /// </summary>
        [JsonProperty("files-wanted")]
        public string[] FilesWanted{ get; set; }

        /// <summary>
        /// Indices of file(s) to download
        /// </summary>
        [JsonProperty("files-unwanted")]
        public string[] FilesUnwanted{ get; set; }

        /// <summary>
        /// Indices of high-priority file(s)
        /// </summary>
        [JsonProperty("priority-high")]
        public string[] PriorityHigh{ get; set; }

        /// <summary>
        /// Indices of low-priority file(s)
        /// </summary>
        [JsonProperty("priority-low")]
        public string[] PriorityLow{ get; set; }

        /// <summary>
        /// Indices of normal-priority file(s)
        /// </summary>
        [JsonProperty("priority-normal")]
        public string[] PriorityNormal{ get; set; }
    }
}
