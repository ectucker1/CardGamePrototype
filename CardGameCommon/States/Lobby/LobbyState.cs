using CardGameCommon.Lobby;
using CardGameCommon.States.Lobby;
using CardGameCommon.States.Playing;
using ProtoBuf;

namespace CardGameCommon.States
{
    [ProtoContract]
    public class LobbyState : IGameState
    {
        [ProtoMember(1)]
        public PlayerList PlayerList { get; private set; } = new PlayerList();

        public bool ValidateMessage(uint source, IMessage message)
        {
            if (source != 0)
                return false;
            
            return true;
        }

        public IGameState HandleMessage(IMessage message)
        {
            switch (message)
            {
                case PlayerJoined joined:
                    PlayerList.AddPlayer(joined.Player);
                    break;
                case PlayerLeft left:
                    PlayerList.RemovePlayer(left.ID);
                    break;
                case GameStart _:
                    return new PlayingState(this);
            }

            return this;
        }

        public IMessage FilterSecrets() => this;
    }
}