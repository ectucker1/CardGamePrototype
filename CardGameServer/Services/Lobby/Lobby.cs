using CardGameServer.Websocket;

namespace CardGameServer.Services.Lobby;

public class Lobby
{
    public string Code { get; private set; }
    
    public GameMessageHandler GameHandler { get; private set; }

    public Lobby(string code)
    {
        Code = code;
        GameHandler = new GameMessageHandler(new ConnectionManager());
    }
}
