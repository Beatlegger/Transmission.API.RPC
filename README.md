Transmission-RPC-API-CSharp
===========================

Transmission RPC API C#

C# implementation of the Transmission RPC API.

| Command              | Not Implemented | Implemented|
| -------------------- |:-:|:-:|
| torrent-start        |   | x |
| torrent-start-now    |   | x |
| torrent-stop         |   | x |
| torrent-verify       |   | x |
| torrent-reannounce   |   | x |
| torrent-set          |   | [UNTESTED] |
| torrent-get          |   | x |
| torrent-add          |   | x |
| torrent-remove       |   | x |
| torrent-set-location |   | x |
| torrent-rename-path  |   | [UNTESTED] |
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

```C#
//Create Transsmission.API.RPC.Client and set host url property (optional set session id property).
Client client = new Client();
client.Host = "http://host:port/transmission/rpc";
client.SessionID = "some_session_id";

//After initialization, the client can call methods:
var sessionInfo = client.GetSession();
var allTorrents = client.TorrentsGetAll(client.AllTorrentsFields);
//<...>
```
