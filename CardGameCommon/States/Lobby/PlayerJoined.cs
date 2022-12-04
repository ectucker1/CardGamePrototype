using CardGameCommon.States;
using ProtoBuf;

namespace CardGameCommon.Lobby
{
    [ProtoContract]
    public class PlayerJoined : IMessage
    {
        [ProtoMember(1)]
        public Player Player { get; set; }

        public IMessage FilterSecrets(uint _) => this;
    }
}