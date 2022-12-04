using System;
using System.Collections.Generic;
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

        public IEnumerable<PlayerMessage> InitServerSecrets() => Array.Empty<PlayerMessage>();
        
        public bool ValidateMessage(uint source, IMessage message)
        {
            switch (message)
            {
                case GameStart start:
                    return true;
                case PlayerJoined joined:
                    return source == 0;
                case PlayerLeft left:
                    return source == 0;
                default:
                    return false;
            }
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

        public IMessage FilterSecrets(uint _) => this;
    }
}