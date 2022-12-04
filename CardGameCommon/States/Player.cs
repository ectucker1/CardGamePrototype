using ProtoBuf;

namespace CardGameCommon.States
{
    [ProtoContract]
    public class Player
    {
        [ProtoMember(1)]
        public uint ID { get; set; }
        
        [ProtoMember(2)]
        public string Name { get; set; }
    }
}