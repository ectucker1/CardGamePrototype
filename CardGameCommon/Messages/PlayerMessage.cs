namespace CardGameCommon
{
    public class PlayerMessage
    {
        public uint PlayerID { get; private set; }
        
        public IMessage Message { get; private set; }

        public PlayerMessage(uint playerId, IMessage message)
        {
            PlayerID = playerId;
            Message = message;
        }
    }
}