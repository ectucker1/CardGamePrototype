using System.Collections.Generic;
using ProtoBuf;

namespace CardGameCommon.States.Playing
{
    [ProtoContract]
    public class Hand
    {
        public uint Player { get; set; }

        public List<Card> Cards { get; set; } = new List<Card>();
    }
}