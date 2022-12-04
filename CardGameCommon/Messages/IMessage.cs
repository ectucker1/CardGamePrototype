namespace CardGameCommon
{
    public interface IMessage
    {
        IMessage FilterSecrets(uint to);
    }
}