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

    protected async Task SendMessageToAsync(uint player, IMessage message, bool filter = false)
    {
        await SendBytesAsync(player, MessageBuilder.WriteMessage(message, filter));
    }
    
    protected async Task SendMessageToAllAsync(IMessage message, bool filter = false)
    {
        await SendBytesToAllAsync(MessageBuilder.WriteMessage(message, filter));
    }

    protected async Task SendSecretMessageToAllAsync(IMessage message, uint owner)
    {
        byte[] unfilteredBytes = MessageBuilder.WriteMessage(message, false);
        byte[] filteredBytes = MessageBuilder.WriteMessage(message, true);
        foreach(var pair in WebSocketConnectionManager.GetAll())
        {
            if (pair.Value.State == WebSocketState.Open)
            {
                await SendBytesAsync(pair.Value, pair.Key == owner ? unfilteredBytes : filteredBytes);
            }
        }
    }
    
    protected async Task HandleThenSendMessageToAllAsync(IMessage message, bool filter = false)
    {
        _gameState = _gameState.HandleMessage(message);
        await SendMessageToAllAsync(message, filter);
        foreach (var init in _gameState.InitServerSecrets())
        {
            await SendMessageToAsync(init.PlayerID, init.Message);
        }
    }

    public override async Task OnConnected(WebSocket socket)
    {
        await base.OnConnected(socket);

        uint playerId = WebSocketConnectionManager.GetId(socket);

        Player player = new Player()
        {
            ID = playerId,
            Name = $"Player {playerId}"
        };
        await HandleThenSendMessageToAllAsync(new PlayerJoined
        {
            Player = player
        });

        await SendMessageToAsync(playerId, _gameState);
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
        var source = WebSocketConnectionManager.GetId(socket);
        var message = MessageBuilder.ReadMessage(buffer);
        
        if (_gameState.ValidateMessage(source, message))
        {
            await HandleThenSendMessageToAllAsync(message);
        }
    }
}
