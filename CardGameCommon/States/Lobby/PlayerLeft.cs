using CardGameCommon.States;
using ProtoBuf;

namespace CardGameCommon.Lobby
{
    [ProtoContract]
    [Message]
    public class PlayerLeft
    {
        [ProtoMember(1)]
        public uint ID { get; set; }
    }
}