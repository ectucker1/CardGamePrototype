using System.Collections.Generic;
using ProtoBuf;

namespace CardGameCommon.States.Playing
{
    [ProtoContract]
    public class Hand
    {
        [ProtoMember(1)]
        public List<Card> Cards { get; set; } = new List<Card>();
    }
}