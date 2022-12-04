using ProtoBuf;

namespace CardGameCommon.States
{
    public interface IGameState
    {
        PlayerList PlayerList { get; }
        
        bool HandleMessage(uint source, object message);
    }
}