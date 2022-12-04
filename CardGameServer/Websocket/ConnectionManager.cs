using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace CardGameServer.Websocket;

// From https://radu-matei.com/blog/aspnet-core-websockets-middleware/
public class ConnectionManager
{
    private object _idLock = new object();
    private uint _nextID = 1;
    
    private ConcurrentDictionary<uint, WebSocket> _sockets = new ConcurrentDictionary<uint, WebSocket>();

    public WebSocket GetSocketById(uint id)
    {
        return _sockets.FirstOrDefault(p => p.Key == id).Value;
    }

    public ConcurrentDictionary<uint, WebSocket> GetAll()
    {
        return _sockets;
    }

    public uint GetId(WebSocket socket)
    {
        return _sockets.FirstOrDefault(p => p.Value == socket).Key;
    }
    public void AddSocket(WebSocket socket)
    {
        _sockets.TryAdd(CreateConnectionId(), socket);
    }

    public async Task RemoveSocket(uint id)
    {
        WebSocket socket;
        _sockets.TryRemove(id, out socket);

        await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
            statusDescription: "Closed by the ConnectionManager",
            cancellationToken: CancellationToken.None);
    }

    private uint CreateConnectionId()
    {
        uint id = 0;
        lock (_idLock)
        {
            id = _nextID;
            _nextID += 1;
        }
        return id;
    }
}
