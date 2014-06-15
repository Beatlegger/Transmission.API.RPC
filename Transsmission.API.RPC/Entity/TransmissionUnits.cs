using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transsmission.API.RPC.Entity
{

    public class TransmissionUnits
    {
        [JsonProperty("speed-units")]
        public string[] SpeedUnits;

        [JsonProperty("speed-bytes")]
        public int SpeedBytes;

        [JsonProperty("size-units")]
        public string[] SizeUnits;

        [JsonProperty("size-bytes")]
        public int SizeBytes;

        [JsonProperty("memory-units")]
        public string[] MemoryUnits;

        [JsonProperty("memory-bytes")]
        public int MemoryBytes;
    }
}
