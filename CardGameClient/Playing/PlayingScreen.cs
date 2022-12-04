using Godot;
using CardGameCommon.States;
using CardGameCommon.States.Lobby;
using CardGameCommon.States.Playing;

public class PlayingScreen : Node
{
    private PackedScene _cardScene;
    private PackedScene _opponentScene;

    private Label _selfName;
    private Label _selfScore;
    private Control _selfHand;

    private Control _opponentBox;

    private Control _playArea;
    
    public override void _Ready()
    {
        _cardScene = GD.Load<PackedScene>("res://Playing/Card.tscn");
        _opponentScene = GD.Load<PackedScene>("res://Playing/Opponent.tscn");

        _selfName = FindNode("SelfName") as Label;
        _selfScore = FindNode("SelfScore") as Label;
        _selfHand = FindNode("SelfHand") as Control;
        
        _opponentBox = FindNode("OpponentBox") as Control;
        
        _playArea = FindNode("PlayArea") as Control;

        Lobby.Instance.Connect(nameof(Lobby.StateUpdated), this, nameof(Lobby.StateUpdated));
        
        UpdateDisplay(Lobby.GameState as PlayingState);
    }

    private Color CardColor(Card card)
    {
        switch (card)
        {
            case Card.RED:
                return Colors.Crimson;
            case Card.GREEN:
                return Colors.ForestGreen;
            case Card.BLUE:
                return Colors.RoyalBlue;
            default:
                return Colors.Wheat;
        }
    }
    
    private void StateUpdated()
    {
        if (Lobby.GameState is PlayingState playingState)
        {
            UpdateDisplay(playingState);
        }
    }

    private void UpdateDisplay(PlayingState state)
    {
        _selfName.Text = state.PlayerList.GetPlayer(Lobby.Instance.SelfID).Name;
        for (int i = 0; i < _selfHand.GetChildCount(); i++)
            _selfHand.GetChild(i).QueueFree();
        foreach (var card in state.Hands[Lobby.Instance.SelfID].Cards)
        {
            var cardDisplay = _cardScene.Instance<ColorRect>();
            cardDisplay.Color = CardColor(card);
            _selfHand.AddChild(cardDisplay);
        }
        
        for (int i = 0; i < _opponentBox.GetChildCount(); i++)
            _opponentBox.GetChild(i).QueueFree();
        foreach (var opponent in state.PlayerList.Players.Values)
        {
            if (opponent.ID != Lobby.Instance.SelfID)
            {
                var opponentDisplay = _opponentScene.Instance<Control>();
                _opponentBox.AddChild(opponentDisplay);
                
                var opponentName = opponentDisplay.FindNode("Name") as Label;
                var opponentScore = opponentDisplay.FindNode("Score") as Label;
                var opponentHand = opponentDisplay.FindNode("Hand") as Control;

                opponentName.Text = opponent.Name;
                foreach (var card in state.Hands[opponent.ID].Cards)
                {
                    var cardDisplay = _cardScene.Instance<ColorRect>();
                    cardDisplay.Color = CardColor(card);
                    opponentHand.AddChild(cardDisplay);
                }
            }
        }
    }
}