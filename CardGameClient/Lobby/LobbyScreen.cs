using Godot;
using CardGameCommon.States;
using CardGameCommon.States.Lobby;
using CardGameCommon.States.Playing;

public class LobbyScreen : Node
{
	private Label _lobbyCode;

	private Button _startGame;
	private Button _quitGame;

	private Control _players;
	private Label _playerTemplate;
	
	public override void _Ready()
	{
		_lobbyCode = FindNode("LobbyCode") as Label;
		_lobbyCode.Text = $"Lobby Code: {Lobby.Instance.LobbyCode}";

		_startGame = FindNode("StartGame") as Button;
		_startGame.Connect("pressed", this, nameof(OnStartPressed));
		
		_quitGame = FindNode("QuitGame") as Button;
		_quitGame.Connect("pressed", this, nameof(OnQuitPressed));

		_players = FindNode("Players") as Control;
		_playerTemplate = _players.GetChild(0) as Label;
		_players.RemoveChild(_playerTemplate);

		Lobby.Instance.Connect(nameof(Lobby.StateUpdated), this, nameof(Lobby.StateUpdated));
		Lobby.GameState = new LobbyState();
	}

	private void StateUpdated()
	{
		UpdatePlayerList();
		if (Lobby.GameState is PlayingState)
		{
			GetTree().ChangeScene("res://Playing/Playing.tscn");
		}
	}

	private void UpdatePlayerList()
	{
		for (int i = 0; i < _players.GetChildCount(); i++)
			_players.GetChild(i).QueueFree();
		
		foreach (Player player in Lobby.GameState.PlayerList.Players.Values)
		{
			Label label = _playerTemplate.Duplicate() as Label;
			label.Text = player.Name;
			label.Visible = true;
			_players.AddChild(label);
		}
	}

	private void OnStartPressed()
	{
		if (Lobby.Instance.ConnectState == LobbyConnectState.JOINED)
		{
			Lobby.Instance.SendMessage(new GameStart());
		}
	}

	private void OnQuitPressed()
	{
		if (Lobby.Instance.ConnectState == LobbyConnectState.JOINED)
		{
			Lobby.Instance.LeaveLobby();
			GetTree().ChangeScene("res://Home/Home.tscn");
		}
	}
}
