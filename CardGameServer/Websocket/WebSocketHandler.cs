using System.Net.WebSockets;
using System.Text;

namespace CardGameServer.Websocket;

// From https://radu-matei.com/blog/aspnet-core-websockets-middleware/
public abstract class WebSocketHandler
{
    protected ConnectionManager WebSocketConnectionManager { get; set; }

    public WebSocketHandler(ConnectionManager webSocketConnectionManager)
    {
        WebSocketConnectionManager = webSocketConnectionManager;
    }

    public virtual async Task OnConnected(WebSocket socket)
    {
        WebSocketConnectionManager.AddSocket(socket);
    }

    public virtual async Task OnDisconnected(WebSocket socket)
    {
        await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));
    }

    public async Task SendBytesAsync(WebSocket socket, byte[] message)
    {
        if(socket.State != WebSocketState.Open)
            return;

        await socket.SendAsync(buffer: new ArraySegment<byte>(array: message,
                offset: 0,
                count: message.Length),
            messageType: WebSocketMessageType.Binary,
            endOfMessage: true,
            cancellationToken: CancellationToken.None);
    }

    public async Task SendBytesAsync(uint socketId, byte[] message)
    {
        await SendBytesAsync(WebSocketConnectionManager.GetSocketById(socketId), message);
    }

    public async Task SendBytesToAllAsync(byte[] message)
    {
        foreach(var pair in WebSocketConnectionManager.GetAll())
        {
            if(pair.Value.State == WebSocketState.Open)
                await SendBytesAsync(pair.Value, message);
        }
    }

    public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
}
