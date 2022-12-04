using CardGameCommon.States;
using ProtoBuf;

namespace CardGameCommon.Lobby
{
    [ProtoContract]
    [Message]
    public class PlayerLeft : IMessage
    {
        [ProtoMember(1)]
        public uint ID { get; set; }

        public IMessage FilterSecrets() => this;
    }
}