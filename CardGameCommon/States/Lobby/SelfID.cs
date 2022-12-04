using ProtoBuf;

namespace CardGameCommon.States.Lobby
{
    [ProtoContract]
    public class SelfID : IMessage
    {
        [ProtoMember(1)]
        public uint PlayerID { get; set; }

        public IMessage FilterSecrets(uint _) => this;
    }
}