using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Transmission.API.RPC.Entity;

namespace Transmission.API.RPC.Common
{
    [JsonSerializable(typeof(Dictionary<string, object>))]
    [JsonSerializable(typeof(JsonElement))]
    [JsonSerializable(typeof(object))]
    [JsonSerializable(typeof(object[]))]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(string[]))]
    [JsonSerializable(typeof(int))]
    [JsonSerializable(typeof(int[]))]
    [JsonSerializable(typeof(long))]
    [JsonSerializable(typeof(long[]))]
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(bool[]))]
    [JsonSerializable(typeof(double))]
    [JsonSerializable(typeof(double[]))]
    [JsonSerializable(typeof(float))]
    [JsonSerializable(typeof(float[]))]
    [JsonSerializable(typeof(decimal))]
    [JsonSerializable(typeof(decimal[]))]
    [JsonSerializable(typeof(Units))]
    [JsonSerializable(typeof(NewTorrentInfo))]
    [JsonSerializable(typeof(RenameTorrentInfo))]
    [JsonSerializable(typeof(SessionInfo))]
    [JsonSerializable(typeof(Statistic))]
    [JsonSerializable(typeof(TransmissionTorrents))]
    internal partial class TransmissionJsonArgumentsContext : JsonSerializerContext
    {
    }
}
