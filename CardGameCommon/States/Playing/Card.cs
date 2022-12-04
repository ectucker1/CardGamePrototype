using ProtoBuf;

namespace CardGameCommon.States.Playing
{
    [ProtoContract]
    public enum Card
    {
        [ProtoEnum]
        UNKOWN = 1,
        [ProtoEnum]
        RED = 2,
        [ProtoEnum]
        GREEN = 3,
        [ProtoEnum]
        BLUE = 4
    }
}