using System;
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

        private bool _serverInit = false;
        
        [ProtoMember(1)]
        public PlayerList PlayerList { get; private set; }

        [ProtoMember(2)]
        public Dictionary<uint, Hand> Hands { get; private set; } = new Dictionary<uint, Hand>();
        
        [ProtoMember(3)]
        public List<Card> Deck { get; private set; }

        [ProtoMember(4)]
        public List<Card> PlayArea { get; private set; } = new List<Card>();

        public PlayingState(LobbyState from)
        {
            PlayerList = from.PlayerList;
            foreach (var player in PlayerList.Players)
            {
                Hands[player.Key] = new Hand()
                {
                    Cards = Enumerable.Repeat(Card.UNKOWN, INITIAL_HAND_SIZE).ToList()
                };
            }

            Deck = Enumerable.Repeat(Card.UNKOWN, INITIAL_DECK_SIZE).ToList();
        }

        private Card RandomCard(Random rand)
        {
            int roll = rand.Next(0, 3);
            switch (roll)
            {
                case 0:
                    return Card.RED;
                case 1:
                    return Card.GREEN;
                case 2:
                    return Card.BLUE;
                default:
                    return Card.RED;
            }
        }
        
        public IEnumerable<PlayerMessage> InitServerSecrets()
        {
            if (!_serverInit)
            {
                Random rand = new Random();
                for (int i = 0; i < Deck.Count; i++)
                {
                    Deck[i] = RandomCard(rand);
                }
            
                foreach (var player in PlayerList.Players.Values)
                {
                    for (int i = 0; i < Hands[player.ID].Cards.Count; i++)
                    {
                        Hands[player.ID].Cards[i] = RandomCard(rand);
                    }

                    yield return new PlayerMessage(player.ID, new SetHand()
                    {
                        Player = player.ID,
                        NewHand = Hands[player.ID]
                    });
                }

                _serverInit = true;
            }
        }
        
        public bool ValidateMessage(uint source, IMessage message)
        {
            switch (message)
            {
                case PlayCard playCard:
                    return source == playCard.PlayerID
                           && playCard.Index >= 0
                           && playCard.Index < Hands[playCard.PlayerID].Cards.Count
                           && Hands[playCard.PlayerID].Cards[playCard.Index] == playCard.Card;
                default:
                    return false;
            }
        }

        public IGameState HandleMessage(IMessage message)
        {
            switch (message)
            {
                case SetHand hand:
                    Hands[hand.Player] = hand.NewHand;
                    break;
                case PlayCard playCard:
                    Hands[playCard.PlayerID].Cards.RemoveAt(playCard.Index);
                    PlayArea.Add(playCard.Card);
                    break;
            }
            return this;
        }
        
        public IMessage FilterSecrets(uint to)
        {
            // TODO filter out deck and hands
            return this;
        }
    }
}