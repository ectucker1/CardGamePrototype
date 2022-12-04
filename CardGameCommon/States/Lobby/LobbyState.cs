using CardGameCommon.Lobby;
using ProtoBuf;

namespace CardGameCommon.States
{
    [ProtoContract]
    [Message]
    public class LobbyState : IGameState
    {
        [ProtoMember(1)]
        public PlayerList PlayerList { get; private set; } = new PlayerList();
        
        public LobbyState()
        {
            
        }

        public bool HandleMessage(uint source, object message)
        {
            switch (message)
            {
                case PlayerJoined joined:
                    if (source != 0) return false;
                    PlayerList.AddPlayer(joined.Player);
                    return true;
                case PlayerLeft left:
                    if (source != 0) return false;
                    PlayerList.RemovePlayer(left.ID);
                    return true;
                default:
                    return false;
            }
        }
    }
}