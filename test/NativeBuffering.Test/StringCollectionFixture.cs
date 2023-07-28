using NativeBuffering;
using NativeBuffering.Collections;
using System.Runtime.CompilerServices;

namespace BufferedMessaging.Test
{
    public class StringCollectionFixture
    {
        [Fact]
        public void GetValue()
        {
            var source = new Source(new string[] { "1", "2", "3" });
            var buffer = new byte[source.CalculateSize()];
            var context = new BufferedObjectWriteContext(buffer);
            source.Write(context);
            var message = Message.Parse(new NativeBuffer(buffer, 0));
            Assert.Equal(3, message.Value.Count);
            Assert.Equal("1", message.Value[0]);
            Assert.Equal("2", message.Value[1]);
            Assert.Equal("3", message.Value[2]);

            Assert.Contains(message.Value, it => it == "1");
            Assert.Contains(message.Value, it => it == "2");
            Assert.Contains(message.Value, it => it == "3");
        }

        public class Source : IBufferedObjectSource
        {
            public Source(string[] value)
            {
                Value = value;
            }

            public string[] Value { get; }
            public int CalculateSize() => Utilities.CalculateCollectionFieldSize(Value);

            public void Write(BufferedObjectWriteContext context)
            {
                using var scope = new BufferedObjectWriteContextScope(context);
                scope.WriteStringCollectionField(Value);
            }
        }

        public unsafe readonly struct Message : IReadOnlyBufferedObject<Message>
        {
            public Message(NativeBuffer buffer)=> Buffer = buffer;
            public NativeBuffer Buffer { get; }
            public ReadOnlyVariableLengthTypeList<BufferedString> Value => Buffer.ReadBufferedObjectCollectionField<BufferedString>(0);
            public static Message Parse(NativeBuffer buffer) => new Message(buffer);
        }
    }
}
