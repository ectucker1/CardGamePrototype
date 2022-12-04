using System.Collections.Generic;
using ProtoBuf;

namespace CardGameCommon.States
{
    [ProtoContract]
    public class PlayerList
    {
        [ProtoMember(1)]
        public Dictionary<uint, Player> Players { get; private set; } = new Dictionary<uint, Player>();

        public void AddPlayer(Player player)
        {
            Players[player.ID] = player;
        }

        public Player GetPlayer(uint id)
        {
            return Players[id];
        }

        public void RemovePlayer(uint id)
        {
            Players.Remove(id);
        }
    }
}