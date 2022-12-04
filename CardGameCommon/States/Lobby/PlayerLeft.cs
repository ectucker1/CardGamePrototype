using CardGameCommon.States;
using ProtoBuf;

namespace CardGameCommon.Lobby
{
    [ProtoContract]
    public class PlayerLeft : IMessage
    {
        [ProtoMember(1)]
        public uint ID { get; set; }

        public IMessage FilterSecrets(uint _) => this;
    }
}