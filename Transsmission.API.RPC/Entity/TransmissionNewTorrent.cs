using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transsmission.API.RPC.Entity
{
    /// <summary>
    /// Information about the torrent file, that will be added
    /// </summary>
    public class TransmissionNewTorrent
    {
        /// <summary>
        /// Pointer to a string of one or more cookies.
        /// </summary>
        [JsonProperty("cookies")]
        public string Cookies;

        /// <summary>
        /// Path to download the torrent to
        /// </summary>
        [JsonProperty("download-dir")]
        public string DownloadDirectory;

        /// <summary>
        /// filename or URL of the .torrent file
        /// </summary>
        [JsonProperty("filename")]
        public string Filename;

        /// <summary>
        /// base64-encoded .torrent content
        /// </summary>
        [JsonProperty("metainfo")]
        public string Metainfo;

        /// <summary>
        /// if true, don't start the torrent
        /// </summary>
        [JsonProperty("paused")]
        public bool Paused;

        /// <summary>
        /// maximum number of peers
        /// </summary>
        [JsonProperty("peer-limit")]
        public int PeerLimit;

        /// <summary>
        /// Torrent's bandwidth priority
        /// </summary>
        [JsonProperty("bandwidthPriority")]
        public int BandwidthPriority;

        /// <summary>
        /// Indices of file(s) to download
        /// </summary>
        [JsonProperty("files-wanted")]
        public string[] FilesWanted;

        /// <summary>
        /// Indices of file(s) to download
        /// </summary>
        [JsonProperty("files-unwanted")]
        public string[] FilesUnwanted;

        /// <summary>
        /// Indices of high-priority file(s)
        /// </summary>
        [JsonProperty("priority-high")]
        public string[] PriorityHigh;

        /// <summary>
        /// Indices of low-priority file(s)
        /// </summary>
        [JsonProperty("priority-low")]
        public string[] PriorityLow;

        /// <summary>
        /// Indices of normal-priority file(s)
        /// </summary>
        [JsonProperty("priority-normal")]
        public string[] PriorityNormal;
    }
}
