Transmission-RPC-API
===========================

![.NET Core](https://github.com/Beatlegger/Transmission.API.RPC/workflows/.NET%20Core/badge.svg)
![Nuget](https://img.shields.io/nuget/v/Transmission.API.RPC)

[Official Transmission RPC specs](https://github.com/transmission/transmission/blob/main/docs/rpc-spec.md) 

C# async client library for the Transmission BitTorrent RPC API. Targets .NET 10.0.

| Command              | Not Implemented | Implemented|
| -------------------- |:-:|:-:|
| torrent-start        |   | x |
| torrent-start-now    |   | x |
| torrent-stop         |   | x |
| torrent-verify       |   | x |
| torrent-reannounce   |   | x |
| torrent-set          |   | x |
| torrent-get          |   | x |
| torrent-add          |   | x |
| torrent-remove       |   | x |
| torrent-set-location |   | x |
| torrent-rename-path  |   | x |
| session-set          |   | x |
| session-get          |   | x |
| session-stats        |   | x |
| blocklist-update     |   | x |
| port-test            |   | x |
| session-close        |   | x |
| queue-move-top       |   | x |
| queue-move-up        |   | x |
| queue-move-down      |   | x |
| queue-move-bottom    |   | x |
| free-space           |   | x |

How to use
-------------

Install Nuget Package: `PM> Install-Package Transmission.API.RPC`

```C#
using Transmission.API.RPC;
using Transmission.API.RPC.Entity;
using Transmission.API.RPC.Arguments;

// URL might look like "schema://host:port/transmission/rpc"
// for example "https://website.com:9091/transmission/rpc"
var client = new Client("URL", sessionID: "PARAM_SESSION_ID", login: "PARAM_LOGIN", password: "PARAM_PASS");

// All methods are async
var sessionInfo = await client.GetSessionInformationAsync();
var allTorrents = await client.TorrentGetAsync(TorrentFields.ALL_FIELDS);

// Add torrent from file
var fileBytes = File.ReadAllBytes("path/to/file.torrent");
var torrent = new NewTorrent
{
    Metainfo = Convert.ToBase64String(fileBytes),
    Paused = true
};
var added = await client.TorrentAddAsync(torrent);

// Add torrent from magnet link
var magnetTorrent = new NewTorrent
{
    Filename = "magnet:?xt=urn:btih:..."
};
var magnetAdded = await client.TorrentAddAsync(magnetTorrent);

// Manage torrents
await client.TorrentStartAsync(new object[] { added.ID });
await client.TorrentStopAsync(new object[] { added.ID });
await client.TorrentRemoveAsync(new int[] { added.ID }, deleteData: false);
```

You can also inject a custom `HttpClient` instance:

```C#
var httpClient = new HttpClient();
var client = new Client("URL", httpClient, login: "PARAM_LOGIN", password: "PARAM_PASS");
```
