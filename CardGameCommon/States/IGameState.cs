using System.Collections.Generic;
using ProtoBuf;

namespace CardGameCommon.States
{
    public interface IGameState : IMessage
    {
        PlayerList PlayerList { get; }

        IEnumerable<PlayerMessage> InitServerSecrets();
        
        bool ValidateMessage(uint source, IMessage message);
        
        IGameState HandleMessage(IMessage message);
    }
}