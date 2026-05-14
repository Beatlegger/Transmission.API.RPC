using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

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
        [RequiresUnreferencedCode("Use a source-generated JsonTypeInfo overload for trimming and Native AOT.")]
        [RequiresDynamicCode("Use a source-generated JsonTypeInfo overload for Native AOT.")]
        public virtual string ToJson()
        {
            return JsonSerializer.Serialize(this, GetType(), _indentedOptions);
        }

        /// <summary>
        /// Deserialize to class
        /// </summary>
        /// <returns></returns>
        [RequiresUnreferencedCode("Use Deserialize(JsonTypeInfo<T>) with source-generated metadata for trimming and Native AOT.")]
        [RequiresDynamicCode("Use Deserialize(JsonTypeInfo<T>) with source-generated metadata for Native AOT.")]
        public T Deserialize<T>()
        {
            var argumentsString = JsonSerializer.Serialize(Arguments);
            return JsonSerializer.Deserialize<T>(argumentsString);
        }

        /// <summary>
        /// Deserialize to class
        /// </summary>
        /// <returns></returns>
        public T Deserialize<T>(JsonTypeInfo<T> jsonTypeInfo)
        {
            var argumentsString = JsonSerializer.Serialize(Arguments, TransmissionJsonArgumentsContext.Default.DictionaryStringObject);
            return JsonSerializer.Deserialize(argumentsString, jsonTypeInfo);
        }
    }
}
