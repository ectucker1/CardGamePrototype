using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CardGameCommon.Lobby;
using ProtoBuf;

namespace CardGameCommon
{
    public static class MessageBuilder
    {
        private static Dictionary<byte, Type> _messageTypes = new Dictionary<byte, Type>();
        private static Dictionary<Type, byte> _messageIds = new Dictionary<Type, byte>();

        static MessageBuilder()
        {
            SortedSet<Type> messageTypes = new SortedSet<Type>(new PredictableTypeComparer());
            Assembly lib = typeof(MessageBuilder).Assembly;
            foreach (Type type in lib.GetTypes())
            {
                MessageAttribute attrib = type.GetCustomAttribute<MessageAttribute>();
                if (attrib != null)
                {
                    messageTypes.Add(type);
                }
            }

            byte id = 1;
            foreach (var type in messageTypes)
            {
                _messageTypes[id] = type;
                _messageIds[type] = id;
                id += 1;
            }
            
            Console.WriteLine($"Registered {messageTypes.Count} message types.");
        }
        
        public static byte[] WriteMessage<T>(T instance)
        {
            MemoryStream stream = new MemoryStream();
            stream.WriteByte(_messageIds[typeof(T)]);
            Serializer.Serialize(stream, instance);
            return stream.ToArray();
        }

        public static object ReadMessage(byte[] message)
        {
            if (message.Length < 2)
                return null;

            Type type = _messageTypes[message[0]];
            MemoryStream protobuf = new MemoryStream(message, 1, message.Length - 1);
            return Serializer.Deserialize(type, protobuf);
        }
        
        private class PredictableTypeComparer : IComparer<Type>
        {
            public int Compare(Type x, Type y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(null, y)) return 1;
                if (ReferenceEquals(null, x)) return -1;
                return string.Compare(x.AssemblyQualifiedName, y.AssemblyQualifiedName, StringComparison.Ordinal);
            }
        }

    }
}