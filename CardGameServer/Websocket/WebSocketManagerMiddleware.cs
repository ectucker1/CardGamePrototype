using System.Net.WebSockets;

namespace CardGameServer.Websocket;

// From https://radu-matei.com/blog/aspnet-core-websockets-middleware/
public class WebSocketManagerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly WebSocketHandler? _webSocketHandler;

    public WebSocketManagerMiddleware(RequestDelegate next,
        WebSocketHandler? handler)
    {
        _next = next;
        _webSocketHandler = handler;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
            return;

        if (_webSocketHandler is null)
            return;

        var socket = await context.WebSockets.AcceptWebSocketAsync();
        await _webSocketHandler.OnConnected(socket);

        await Receive(socket, async (result, buffer) =>
        {
            if(result.MessageType == WebSocketMessageType.Binary)
            {
                await _webSocketHandler.ReceiveAsync(socket, result, buffer);
            }
            else if(result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocketHandler.OnDisconnected(socket);
            }
        });
    }

    private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        var buffer = new byte[1024 * 4];

        while(socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                cancellationToken: CancellationToken.None);

            handleMessage(result, buffer);
        }
    }
}
