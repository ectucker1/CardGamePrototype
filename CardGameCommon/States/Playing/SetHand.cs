using System.Collections.Generic;
using ProtoBuf;

namespace CardGameCommon.States.Playing
{
    [ProtoContract]
    public class SetHand : IMessage
    {
        [ProtoMember(1)]
        public uint Player { get; set; }
        
        [ProtoMember(2)]
        public Hand NewHand { get; set; }
        
        public IMessage FilterSecrets(uint _) => this;
    }
}