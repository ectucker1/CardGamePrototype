using ProtoBuf;

namespace CardGameCommon.States
{
    public interface IGameState : IMessage
    {
        PlayerList PlayerList { get; }

        bool ValidateMessage(uint source, IMessage message);
        
        IGameState HandleMessage(IMessage message);
    }
}