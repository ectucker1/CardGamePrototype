using ProtoBuf;

namespace CardGameCommon.States.Lobby
{
    [ProtoContract]
    [Message]
    public class GameStart : IMessage
    {
        public IMessage FilterSecrets() => this;
    }
}