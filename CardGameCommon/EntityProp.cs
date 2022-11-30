using MessagePack;

namespace CardGameCommon
{
    [MessagePack.Union(0, typeof(Prop<int>))]
    [MessagePack.Union(1, typeof(Prop<string>))]
    public interface IGenericProp
    {
        
    }
    
    [MessagePackObject]
    public class Prop<T> : IGenericProp
    {
        [Key(0)]
        public T Value { get; set; }

        private Prop() {}

        public static Prop<T> Create(T val)
        {
            var prop = new Prop<T>()
            {
                Value = val
            };
            return prop;
        }
    }
}