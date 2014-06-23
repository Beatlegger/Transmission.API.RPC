using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.API.RPC.Common
{
    public abstract class TransmissionRequestResponse
    {
        /// <summary>
        /// Data
        /// </summary>
        [JsonProperty("arguments")]
        public Dictionary<string, object> Arguments;

        /// <summary>
        /// Number (id)
        /// </summary>
        [JsonProperty("tag")]
        public int Tag;

        /// <summary>
        /// Convert to JSON string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Deserialize to class
        /// </summary>
        /// <returns></returns>
        public T Deserialize<T>()
        {
            var argumentsString = JsonConvert.SerializeObject(this.Arguments);
            return JsonConvert.DeserializeObject<T>(argumentsString);
        }
    }

    /// <summary>
    /// Transmission request 
    /// </summary>
    public class TransmissionRequest : TransmissionRequestResponse
    {
        /// <summary>
        /// Name of the method to invoke
        /// </summary>
        [JsonProperty("method")]
        public string Method;

        public TransmissionRequest(string method, Dictionary<string, object> arguments)
        {
            this.Method = method;
            this.Arguments = arguments;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    /// <summary>
    /// Transmission response 
    /// </summary>
    public class TransmissionResponse : TransmissionRequestResponse
    {
        /// <summary>
        /// Contains "success" on success, or an error string on failure.
        /// </summary>
        [JsonProperty("result")]
        public string Result;
    }
}
