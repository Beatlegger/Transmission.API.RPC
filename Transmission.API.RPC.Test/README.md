# Transmission.API.RPC.Test

Интеграционный тест-набор для `Transmission.API.RPC`. Требует живой Transmission-демон — всё гоняется против реального RPC.

## Запуск

```bash
dotnet test                                   # все тесты
dotnet test --filter "MethodName"             # один тест по имени
dotnet test --filter "FullyQualifiedName!~Blocklist"  # исключить подстроку
```

### Требования

- Запущенный Transmission с включённым RPC по адресу `http://localhost:9091/transmission/rpc` (без авторизации).
- Исходящий интернет для `BlocklistUpdate_Test` — тест временно подставляет публичный `list.iblocklist.com` и восстанавливает исходные настройки в `finally`.
- Файл `Data/ubuntu-10.04.4-server-amd64.iso.torrent` копируется в output автоматически через csproj.

## Структура

Единственный файл тестов — `MethodsTest.cs`. Внутри один класс `MethodsTest`, один shared-fixture `TransmissionFixture` и одна коллекция `Integration` (с `DisableParallelization = true`). Отдельного unit-проекта нет — unit-покрытие (мок `HttpMessageHandler`) — это отдельная задача, если понадобится.

### TransmissionFixture

`ICollectionFixture<TransmissionFixture>` через `[CollectionDefinition("Integration")]`. В `InitializeAsync` добавляет Ubuntu-торрент в режиме `Paused` и экспонирует `TorrentId` / `TorrentHashString`. В `DisposeAsync` удаляет торрент с `deleteData: true`, обёрнуто в `try/catch` — устойчиво к любым ошибкам уборки.

Клиент фикстуры создаётся как `new Client(HOST)` — **без** кастомного `HttpClient`. Это принципиально: `Client.cs` собирает дефолтный `HttpClient` c `SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.Zero }`, отключая пул соединений. Transmission агрессивно закрывает keep-alive-соединения (особенно после `blocklist-update`), и без этого тесты ловили `HttpIOException: ResponseEnded` на стадиях, где пул выдавал уже мёртвый сокет. Если кастомизировать `HttpClient` в фикстуре, нужно явно ставить `PooledConnectionLifetime = TimeSpan.Zero`, иначе флейки вернутся.

## Сводка по тестам — `MethodsTest`

Всего 25 `[Fact]`, разбитых на регионы.

### Torrent CRUD

- **`AddTorrent_Test`** — проверяет, что фикстура добавила торрент (`TorrentId != 0`, `HashString != null`).
- **`GetTorrentInfo_Test`** — `TorrentGetAsync(ALL_FIELDS)` возвращает непустой массив торрентов.
- **`GetTorrentById_Test`** — фильтрация по `_fixture.TorrentId` возвращает ровно нужный торрент.
- **`SetTorrentSettings_Test`** — удаление трекера через `TorrentSetAsync` уменьшает `Trackers.Length`. Использует устаревший `TrackerRemove` (помечено `[Obsolete]` в API — warning ожидаем).
- **`RenamePathTorrent_Test`** — `TorrentRenamePathAsync` переименовывает первый файл, затем восстанавливает исходное имя.
- **`RemoveTorrent_Test`** — добавляет отдельный торрент через magnet-ссылку со **случайным SHA-1 хэшем**, чтобы не получить `torrent-duplicate` с ID fixture-торрента. Проверяет, что ID отличается от fixture, удаляет и убеждается, что он исчез из списка.

### Сессия

- **`SessionGet_Test`** — `GetSessionInformationAsync` возвращает `SessionInfo` с непустым `Version`.
- **`ChangeSession_Test`** — сохраняет `SpeedLimitUp`, ставит в 100, проверяет, восстанавливает исходное значение.
- **`GetSessionStatistic_Test`** — `GetSessionStatisticAsync` возвращает неотрицательные `ActiveTorrentCount` / `downloadSpeed` / `uploadSpeed`.
- **`BlocklistUpdate_Test`** — сохраняет `BlocklistURL` / `BlocklistEnabled`, подставляет публичный `list.iblocklist.com`, вызывает `BlocklistUpdateAsync` (ожидает `>= 0`), в `finally` восстанавливает настройки. Требует исходящего интернета. Запускает тяжёлую операцию на демоне — именно поэтому `Client` использует retry с увеличенным backoff, иначе следующие тесты иногда ловили `ResponseEnded`.
- **`FreeSpace_Test`** — `FreeSpaceAsync("/")` возвращает `>= 0`.
- **`PortTest_Test`** — `PortTestAsync` возвращает `bool` (значение зависит от сети; тест проверяет, что вызов отработал).

### Очередь

- **`TorrentQueueMoveTop_Test`**, **`TorrentQueueMoveUp_Test`**, **`TorrentQueueMoveDown_Test`**, **`TorrentQueueMoveBottom_Test`** — вызывают соответствующие `queue-move-*` методы для fixture-торрента; проверка — отсутствие исключений (smoke).

### Управление торрентом

- **`TorrentSetLocation_Test`** — Transmission требует абсолютный путь, поэтому тест берёт текущий `downloadDir` из `session-get` и вызывает `TorrentSetLocationAsync` с `move: false` (no-op без побочных эффектов).
- **`TorrentStart_Test` / `TorrentStop_Test` / `TorrentStartNow_Test` / `TorrentVerify_Test`** — smoke-тесты одноимённых RPC для fixture-торрента.
- **`TorrentStartAll_Test` / `TorrentStopAll_Test` / `TorrentStartNowAll_Test` / `TorrentVerifyAll_Test`** — безаргументные перегрузки (вызываются с `ids: "recently-active"`).

## Что **не** покрыто — `session-close`

Ранее в наборе был `CloseSession_Test`, который дёргал `CloseSessionAsync`. Он удалён — **не потому что session-close гасит демон** (эмпирически на Transmission 4.1.1 демон после RPC продолжает работать), а потому что он инвалидирует server-side состояние сессии так, что последующие `torrent-get by id` в том же shared fixture начинают сыпать пустые ответы. 3 из 5 прогонов падали на `SetTorrentSettings_Test` / `GetTorrentById_Test` / `RenamePathTorrent_Test` с `Assert.NotNull(torrentInfo)` — фикстурный торрент "пропадал" для запросов по ID.

Решение: `CloseSessionAsync` после RPC вызывает `ResetSessionId()` (сбрасывает кэшированный `X-Transmission-Session-Id`, чтобы следующий запрос прошёл новый handshake), а сам интеграционный smoke-тест убран. Проверять, что метод действительно шлёт `session-close` RPC, правильно в unit-тесте с мокнутым `HttpMessageHandler` — для него нужно завести отдельный проект.

В коде библиотеки есть **два независимых метода**:

- `Client.ResetSessionId()` — чисто клиентская операция: сбрасывает кэшированный CSRF-токен, никаких RPC не шлёт, демон не трогает. Нужно, если хочется форсировать новый handshake (например, после смены кредов).
- `Client.CloseSessionAsync()` — шлёт RPC `session-close` и затем зовёт `ResetSessionId()` внутри, чтобы не ходить с инвалидированным токеном.

## Устойчивость HTTP-слоя

Интеграционные тесты раньше флейкали `HttpIOException: The response ended prematurely. (ResponseEnded)` — каждое по-одиночке проходило, а пачкой валилось. Причина: HttpClient переиспользовал keep-alive соединения, которые Transmission уже закрыл со своей стороны. Текущее состояние:

1. **`Client.cs`** — дефолтный `HttpClient` собирается с `SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.Zero }`. Пул выключен — каждый запрос идёт по свежему TCP.
2. **`Client.Async.cs`** — на каждом `HttpRequestMessage` ставится `Headers.ConnectionClose = true` (дополнительная страховка, чтобы сервер не держал соединение).
3. **`Client.Async.cs`** — retry-цикл внутри `SendRequestAsync` ловит и `HttpRequestException`, и `IOException` (HttpIOException наследуется от `IOException` и может вылетать прямо из `ReadAsStringAsync`, минуя `HttpRequestException`). `maxRetries = 5`, backoff `250 * 2^retry` мс — итого ~7.5 секунд, достаточно чтобы пережить временную неотзывчивость демона (например, во время `blocklist-update`).

Если добавляете новые интеграционные тесты, которые запускают тяжёлые операции на демоне (долгий fetch, indexing, проверки), — не надо трогать эти настройки, ретраи уже всё покрывают.

## Стабильность

5 прогонов `dotnet test` подряд, 25/25 зелёные каждый раз. Длительность — 2–11 секунд (зависит от `BlocklistUpdate_Test`, ему нужно скачать список с iblocklist.com).
