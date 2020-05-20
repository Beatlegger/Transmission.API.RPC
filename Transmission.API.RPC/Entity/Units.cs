using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.API.RPC.Entity
{
    /// <summary>
    /// Units
    /// </summary>
    public class Units
    {
        /// <summary>
        /// Speed units
        /// </summary>
        [JsonProperty("speed-units")]
        public string[] SpeedUnits { get; set; }

        /// <summary>
        /// Speed bytes
        /// </summary>
        [JsonProperty("speed-bytes")]
        public int? SpeedBytes { get; set; }

        /// <summary>
        /// Size units
        /// </summary>
        [JsonProperty("size-units")]
        public string[] SizeUnits { get; set; }

        /// <summary>
        /// Size bytes
        /// </summary>
        [JsonProperty("size-bytes")]
        public int? SizeBytes { get; set; }

        /// <summary>
        /// Memory units
        /// </summary>
        [JsonProperty("memory-units")]
        public string[] MemoryUnits { get; set; }

        /// <summary>
        /// Memory bytes
        /// </summary>
        [JsonProperty("memory-bytes")]
        public int? MemoryBytes { get; set; }
    }
}
