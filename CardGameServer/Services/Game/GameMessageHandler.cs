using System.Net.WebSockets;
using System.Text;
using CardGameCommon;
using CardGameCommon.Lobby;
using CardGameCommon.States;
using CardGameServer.Websocket;

namespace CardGameServer.Services;

public class GameMessageHandler : WebSocketHandler
{
    private IGameState _gameState = new LobbyState();
    
    public GameMessageHandler(ConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager) { }

    private bool HandleMessage(uint source, object message)
    {
        return _gameState.HandleMessage(0, message);
    }

    private async Task HandleThenSendMessageToAllAsync<T>(T message)
    {
        if (HandleMessage(0, message))
        {
            await SendMessageToAllAsync(MessageBuilder.WriteMessage(message));
        }
    }
    
    public override async Task OnConnected(WebSocket socket)
    {
        await base.OnConnected(socket);

        uint socketId = WebSocketConnectionManager.GetId(socket);

        Player player = new Player()
        {
            ID = socketId,
            Name = $"Player {socketId}"
        };
        await HandleThenSendMessageToAllAsync(new PlayerJoined
        {
            Player = player
        });

        switch (_gameState)
        {
            case LobbyState lobbyState:
                await SendMessageAsync(socketId, MessageBuilder.WriteMessage(lobbyState));
                break;
        }
    }

    public override async Task OnDisconnected(WebSocket socket)
    {
        uint id = WebSocketConnectionManager.GetId(socket);
        await base.OnDisconnected(socket);

        await HandleThenSendMessageToAllAsync(new PlayerLeft()
        {
            ID = id
        });
    }

    public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
    {
        var socketId = WebSocketConnectionManager.GetId(socket);
        var message = MessageBuilder.ReadMessage(buffer);
    }
}
