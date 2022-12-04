using CardGameCommon.States;
using ProtoBuf;

namespace CardGameCommon.Lobby
{
    [ProtoContract]
    [Message]
    public class PlayerJoined : IMessage
    {
        [ProtoMember(1)]
        public Player Player { get; set; }

        public IMessage FilterSecrets() => this;
    }
}