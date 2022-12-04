using System.Text;
using CardGameCommon;
using CardGameCommon.States;
using CardGameCommon.States.Lobby;
using CardGameCommon.States.Playing;
using Godot;

public enum LobbyConnectState
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
    
    public LobbyConnectState ConnectState { get; private set; }
    
    public string LobbyCode { get; private set; }
    
    public uint SelfID { get; private set; }

    [Signal]
    public delegate void Created();

    [Signal]
    public delegate void Joined();

    [Signal]
    public delegate void ConnectFailed(string error);

    [Signal]
    public delegate void StateUpdated();

    public static IGameState GameState { get; set; }

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
        if (ConnectState == LobbyConnectState.NONE)
        {
            _lobbyCreateRequest.Request(LOBBY_CREATE_URL);
            ConnectState = LobbyConnectState.CREATING;
        }
    }

    public void JoinLobby(string code)
    {
        if (ConnectState == LobbyConnectState.NONE || ConnectState == LobbyConnectState.CREATED)
        {
            ConnectState = LobbyConnectState.JOINING;
            string url = $"{WS_BASE_URL}/game/{code}";
            var connected = (bool)_socketClient.Call("connect_url", url);
            if (!connected)
            {
                ConnectState = LobbyConnectState.NONE;
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
        ConnectState = LobbyConnectState.NONE;
        LobbyCode = "";
        _socketClient.Call("disconnect_host");
    }

    public void SendMessage(IMessage message)
    {
        if (ConnectState == LobbyConnectState.JOINED)
        {
            byte[] bytes = MessageBuilder.WriteMessage(message);
            _socketClient.Call("send_bytes", bytes);
        }
    }
    
    private void _OnCreateLobbyRequestCompleted(int result, int responseCode, string[] headers, byte[] body)
    {
        if (responseCode == 200)
        {
            LobbyCode = Encoding.UTF8.GetString(body);
            ConnectState = LobbyConnectState.CREATED;
            EmitSignal(nameof(Created));
            JoinLobby(LobbyCode);
        }
        else
        {
            EmitSignal(nameof(ConnectFailed), "Could not create lobby.");
            ConnectState = LobbyConnectState.NONE;
        }
    }

    private void _SocketClosed(bool clean = false)
    {
        ConnectState = LobbyConnectState.NONE;
        EmitSignal(nameof(ConnectFailed), "Disconnected.");
    }

    private void _SocketConnected(string proto = "")
    {
        ConnectState = LobbyConnectState.JOINED;
        EmitSignal(nameof(Joined));
    }

    private void _SocketData(byte[] data)
    {
        if (ConnectState == LobbyConnectState.JOINED)
        {
            var message = MessageBuilder.ReadMessage(data);
            GD.Print("Received message of type" + message.GetType().FullName);
            if (message is IGameState state)
            {
                GameState = state;
                EmitSignal(nameof(StateUpdated));
            }
            else if (message is SelfID selfId)
            {
                SelfID = selfId.PlayerID;
                EmitSignal(nameof(StateUpdated));
            }
            else if (GameState != null)
            {
                GameState = GameState.HandleMessage(message);
                EmitSignal(nameof(StateUpdated));
            }
        }
    }
}