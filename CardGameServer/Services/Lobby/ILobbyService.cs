namespace CardGameServer.Services.Lobby;

public interface ILobbyService
{
    public Lobby CreateLobby();

    public Lobby? GetLobby(string code);
    
    public GameMessageHandler? GetMessageHandler(string code);
}
