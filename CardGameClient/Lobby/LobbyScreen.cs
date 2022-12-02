using Godot;
using System;

public class LobbyScreen : Node
{
	private Label _lobbyCode;

	private Button _startGame;
	private Button _quitGame;
	
	public override void _Ready()
	{
		_lobbyCode = FindNode("LobbyCode") as Label;
		_lobbyCode.Text = $"Lobby Code: {Lobby.Instance.LobbyCode}";

		_startGame = FindNode("StartGame") as Button;
		_startGame.Connect("pressed", this, nameof(OnStartPressed));
		
		_quitGame = FindNode("QuitGame") as Button;
		_quitGame.Connect("pressed", this, nameof(OnQuitPressed));
	}

	private void OnStartPressed()
	{
		if (Lobby.Instance.State == LobbyState.JOINED)
		{
		}
	}

	private void OnQuitPressed()
	{
		if (Lobby.Instance.State == LobbyState.JOINED)
		{
			Lobby.Instance.LeaveLobby();
			GetTree().ChangeScene("res://Home/Home.tscn");
		}
	}
}
