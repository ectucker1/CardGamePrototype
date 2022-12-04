using Godot;
using System;
using System.Text;

public class HomeScreen : Control
{
    private Button _hostBtn;
    private Button _joinBtn;

    private LineEdit _lobbyCodeEdit;
    
    public override void _Ready()
    {
        base._Ready();

        _hostBtn = FindNode("HostGame") as Button;
        _hostBtn.Connect("pressed", this, nameof(_OnHostPressed));
        
        _joinBtn = FindNode("JoinGame") as Button;
        _joinBtn.Connect("pressed", this, nameof(_OnJoinPressed));
        
        _lobbyCodeEdit = FindNode("LobbyCode") as LineEdit;
        _lobbyCodeEdit.Connect("text_changed", this, nameof(CodeChanged));
        
        Lobby.Instance.Connect(nameof(Lobby.Joined), this, nameof(GoToLobby));
        Lobby.Instance.Connect(nameof(Lobby.ConnectFailed), this, nameof(ShowError));
    }

    private void _OnHostPressed()
    {
        Lobby.Instance.CreateLobby();
        UpdateButtonsEnabled();
    }

    private void _OnJoinPressed()
    {
        Lobby.Instance.JoinLobby(_lobbyCodeEdit.Text.ToUpper());
        UpdateButtonsEnabled();
    }

    private void GoToLobby()
    {
        GetTree().ChangeScene("res://Lobby/Lobby.tscn");
    }

    private void ShowError(string error)
    {
        UpdateButtonsEnabled();
    }

    private void CodeChanged(string error)
    {
        UpdateButtonsEnabled();
    }

    private void UpdateButtonsEnabled()
    {
        switch (Lobby.Instance.ConnectState)
        {
            case LobbyConnectState.NONE:
                _hostBtn.Disabled = false;
                _joinBtn.Disabled = _lobbyCodeEdit.Text.Length != 5;
                break;
            default:
                _hostBtn.Disabled = true;
                _joinBtn.Disabled = true;
                break;
        }
    }
}
