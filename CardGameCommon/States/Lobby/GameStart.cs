using ProtoBuf;

namespace CardGameCommon.States.Lobby
{
    [ProtoContract]
    public class GameStart : IMessage
    {
        public IMessage FilterSecrets() => this;
    }
}