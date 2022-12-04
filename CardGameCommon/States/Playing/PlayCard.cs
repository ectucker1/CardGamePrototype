using ProtoBuf;

namespace CardGameCommon.States.Playing
{
    [ProtoContract]
    public class PlayCard : IMessage
    {
        [ProtoMember(1)]
        public uint PlayerID { get; set; }
        
        [ProtoMember(2)]
        public int Index { get; set; }
        
        [ProtoMember(3)]
        public Card Card { get; set; }

        public IMessage FilterSecrets(uint _) => this;
    }
}