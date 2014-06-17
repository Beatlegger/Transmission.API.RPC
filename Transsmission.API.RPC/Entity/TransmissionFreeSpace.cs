using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transsmission.API.RPC.Entity
{
    public class TransmissionFreeSpace
    {
        [JsonProperty("path")]
        public string Path{ get; set; }

        [JsonProperty("size-bytes")]
        public long SizeBytes{ get; set; }
    }
}
