using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Transmission.API.RPC.Common
{
    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(TransmissionResponse))]
    [JsonSerializable(typeof(Dictionary<string, object>))]
    [JsonSerializable(typeof(JsonElement))]
    [JsonSerializable(typeof(object))]
    internal partial class TransmissionJsonResponseContext : JsonSerializerContext
    {
    }
}
