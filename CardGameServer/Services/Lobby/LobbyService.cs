using System.Collections.Concurrent;
using System.Text;

namespace CardGameServer.Services.Lobby;

public class LobbyService : ILobbyService
{
    private ConcurrentDictionary<string, Lobby> _lobbies = new();

    private Random _rand = new();

    private string GenerateCode()
    {
        lock (_rand)
        {
            const string characters = "ABCDEFGHJKMNP23456789";
            const int count = 5;
            StringBuilder builder = new StringBuilder(count);
            for (int i = 0; i < count; i++)
            {
                builder.Append(characters[_rand.Next() % characters.Length]);
            }
            return builder.ToString();
        }
    }
    
    public Lobby CreateLobby()
    {
        string code = GenerateCode();
        Lobby lobby = new Lobby(code);
        _lobbies[code] = lobby;
        return lobby;
    }
    
    public Lobby? GetLobby(string code)
    {
        return _lobbies[code];
    }
    
    public GameMessageHandler? GetMessageHandler(string code)
    {
        return GetLobby(code)?.GameHandler;
    }
}