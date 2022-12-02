using System.Text;
using Godot;

public enum LobbyState
{
    NONE,
    CREATING,
    CREATED,
    JOINING,
    JOINED
}

public class Lobby : Node
{
    private const string HTTP_BASE_URL = "http://localhost:5183";
    private const string LOBBY_CREATE_URL = HTTP_BASE_URL + "/lobby/create";

    private const string WS_BASE_URL = "ws://localhost:5183";
    
    private HTTPRequest _lobbyCreateRequest;

    private Node _socketClient;

    private static Lobby _instance;
    public static Lobby Instance => _instance;
    
    public LobbyState State { get; private set; }
    
    public string LobbyCode { get; private set; }

    [Signal]
    public delegate void Created();

    [Signal]
    public delegate void Joined();

    [Signal]
    public delegate void ConnectFailed(string error);

    public override void _Ready()
    {
        base._Ready();

        _instance = this;
        
        _lobbyCreateRequest = FindNode("LobbyCreateRequest") as HTTPRequest;
        _lobbyCreateRequest.Connect("request_completed", this, nameof(_OnCreateLobbyRequestCompleted));

        _socketClient = FindNode("SocketClient");
        _socketClient.Connect("closed", this, nameof(_SocketClosed));
        _socketClient.Connect("established", this, nameof(_SocketConnected));
        _socketClient.Connect("data_received", this, nameof(_SocketData));
    }

    public void CreateLobby()
    {
        if (State == LobbyState.NONE)
        {
            _lobbyCreateRequest.Request(LOBBY_CREATE_URL);
            State = LobbyState.CREATING;
        }
    }

    public void JoinLobby(string code)
    {
        if (State == LobbyState.NONE || State == LobbyState.CREATED)
        {
            State = LobbyState.JOINING;
            string url = $"{WS_BASE_URL}/game/{code}";
            var connected = (bool)_socketClient.Call("connect_url", url);
            if (!connected)
            {
                State = LobbyState.NONE;
                EmitSignal(nameof(ConnectFailed), "Could not join lobby");
            }
            else
            {
                LobbyCode = code;
            }
        }
    }

    public void LeaveLobby()
    {
        State = LobbyState.NONE;
        LobbyCode = "";
        _socketClient.Call("disconnect_host");
    }
    
    private void _OnCreateLobbyRequestCompleted(int result, int responseCode, string[] headers, byte[] body)
    {
        if (responseCode == 200)
        {
            LobbyCode = Encoding.UTF8.GetString(body);
            State = LobbyState.CREATED;
            EmitSignal(nameof(Created));
            JoinLobby(LobbyCode);
        }
        else
        {
            EmitSignal(nameof(ConnectFailed), "Could not create lobby.");
            State = LobbyState.NONE;
        }
    }

    private void _SocketClosed(bool clean = false)
    {
        State = LobbyState.NONE;
        EmitSignal(nameof(ConnectFailed), "Disconnected.");
    }

    private void _SocketConnected(string proto = "")
    {
        State = LobbyState.JOINED;
        EmitSignal(nameof(Joined));
    }

    private void _SocketData(byte[] data)
    {
        string packet = data.GetStringFromUTF8();
    }
}