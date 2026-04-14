using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Transmission.API.RPC.Common
{
    /// <summary>
    /// Base class for request/response
    /// </summary>
    public abstract class CommunicateBase
    {
        private static readonly JsonSerializerOptions _indentedOptions = new()
        {
            WriteIndented = true
        };

        /// <summary>
        /// Data
        /// </summary>
        [JsonPropertyName("arguments")]
        public Dictionary<string, object> Arguments { get; set; }

        /// <summary>
        /// Number (id)
        /// </summary>
        [JsonPropertyName("tag")]
        public int Tag { get; set; }

        /// <summary>
        /// Convert to JSON string
        /// </summary>
        /// <returns></returns>
        public virtual string ToJson()
        {
            return JsonSerializer.Serialize(this, GetType(), _indentedOptions);
        }

        /// <summary>
        /// Deserialize to class
        /// </summary>
        /// <returns></returns>
        public T Deserialize<T>()
        {
            var argumentsString = JsonSerializer.Serialize(Arguments);
            return JsonSerializer.Deserialize<T>(argumentsString);
        }
    }
}
