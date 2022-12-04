using System.Collections.Generic;
using System.Linq;
using CardGameCommon.Lobby;
using ProtoBuf;

namespace CardGameCommon.States.Playing
{
    [ProtoContract]
    public class PlayingState : IGameState
    {
        private const int INITIAL_HAND_SIZE = 5;
        private const int INITIAL_DECK_SIZE = 5;
        
        [ProtoMember(1)]
        public PlayerList PlayerList { get; private set; }
        
        [ProtoMember(2)]
        public Dictionary<uint, Hand> Hands { get; private set; }
        
        [ProtoMember(3)]
        public List<Card> Deck { get; private set; }

        public PlayingState(LobbyState from)
        {
            PlayerList = from.PlayerList;
            foreach (var player in PlayerList.Players)
            {
                Hands[player.Key] = new Hand()
                {
                    Player = player.Key,
                    Cards = Enumerable.Repeat(Card.UNKOWN, INITIAL_HAND_SIZE).ToList()
                };
            }

            Deck = Enumerable.Repeat(Card.UNKOWN, INITIAL_DECK_SIZE).ToList();
        }
        
        public bool ValidateMessage(uint source, IMessage message)
        {
            return true;
        }

        public IGameState HandleMessage(IMessage message)
        {
            return this;
        }


        public IMessage FilterSecrets() => this;
    }
}