using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Transmission.API.RPC.Arguments;

namespace Transmission.API.RPC.Entity
{
    public class SessionInfo : SessionSettings
    {
        /// <summary>
        /// Number of rules in the blocklist
        /// </summary>
        [JsonProperty("blocklist-size")]
        public int BlocklistSize{ get; set; }

        /// <summary>
        /// Location of transmission's configuration directory
        /// </summary>
        [JsonProperty("config-dir")]
        public string ConfigDirectory{ get; set; }

        /// <summary>
        /// The current RPC API version
        /// </summary>
        [JsonProperty("rpc-version")]
        public int RpcVersion{ get; set; }

        /// <summary>
        /// The minimum RPC API version supported
        /// </summary>
        [JsonProperty("rpc-version-minimum")]
        public int RpcVersionMinimum{ get; set; }

        /// <summary>
        /// Long version string "$version ($revision)"
        /// </summary>
        [JsonProperty("version")]
        public string Version{ get; set; }
    }
}
