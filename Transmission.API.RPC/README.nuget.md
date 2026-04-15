# Transmission.API.RPC

C# async client library for the [Transmission RPC API](https://github.com/transmission/transmission/blob/main/docs/rpc-spec.md). Targets **.NET 10.0**.

Full documentation and source code: https://github.com/Beatlegger/Transmission.API.RPC

## Supported RPC methods

All methods are implemented. See the GitHub README for the full feature matrix.

## Quick start

```csharp
using Transmission.API.RPC;
using Transmission.API.RPC.Entity;
using Transmission.API.RPC.Arguments;

// URL format: "schema://host:port/transmission/rpc"
// e.g. "https://example.com:9091/transmission/rpc"
var client = new Client(
    "URL",
    sessionID: "PARAM_SESSION_ID",
    login: "PARAM_LOGIN",
    password: "PARAM_PASS");

// All methods are async
var sessionInfo = await client.GetSessionInformationAsync();
var allTorrents = await client.TorrentGetAsync(TorrentFields.ALL_FIELDS);
```

## Adding torrents

```csharp
// From a .torrent file
var fileBytes = File.ReadAllBytes("path/to/file.torrent");
var torrent = new NewTorrent
{
    Metainfo = Convert.ToBase64String(fileBytes),
    Paused = true
};
var added = await client.TorrentAddAsync(torrent);

// From a magnet link
var magnetTorrent = new NewTorrent
{
    Filename = "magnet:?xt=urn:btih:..."
};
var magnetAdded = await client.TorrentAddAsync(magnetTorrent);
```

## Managing torrents

```csharp
await client.TorrentStartAsync(new object[] { added.ID });
await client.TorrentStopAsync(new object[] { added.ID });
await client.TorrentRemoveAsync(new int[] { added.ID }, deleteData: false);
```

## Custom HttpClient

```csharp
var httpClient = new HttpClient();
var client = new Client("URL", httpClient, login: "PARAM_LOGIN", password: "PARAM_PASS");
```

## License

MIT
