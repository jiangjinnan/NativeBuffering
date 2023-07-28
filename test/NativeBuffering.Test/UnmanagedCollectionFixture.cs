using NativeBuffering;
using NativeBuffering.Collections;
using System.Runtime.CompilerServices;

namespace BufferedMessaging.Test
{
    public class UnmanagedCollectionFixture
    {
        [Fact]
        public void GetValue()
        {
            var source = new Source(new long[] { 1, 2, 3 });
            var buffer = new byte[source.CalculateSize()];
            var context = new BufferedObjectWriteContext(buffer);
            source.Write(context);
            var message = Message.Parse(new NativeBuffer(buffer, 0));
            Assert.Equal(3, message.Value.Count);
            Assert.Equal(1, message.Value[0]);
            Assert.Equal(2, message.Value[1]);
            Assert.Equal(3, message.Value[2]);

            Assert.Contains(message.Value, it => it == 1);
            Assert.Contains(message.Value, it => it == 2);
            Assert.Contains(message.Value, it => it == 3);
        }

        public class Source : IBufferedObjectSource
        {
            public Source(long[] value)
            {
                Value = value;
            }

            public long[] Value { get; }
            public int CalculateSize() => Utilities.CalculateCollectionFieldSize(Value);

            public void Write(BufferedObjectWriteContext context)
            {
                using var scope = new BufferedObjectWriteContextScope(context);
                scope.WriteUnmanagedCollectionField(Value);
            }
        }

        public unsafe readonly struct Message : IReadOnlyBufferedObject<Message>
        {
            public Message(NativeBuffer buffer)=> Buffer = buffer;
            public NativeBuffer Buffer { get; }
            public ReadOnlyFixedLengthTypedList<long> Value => Buffer.ReadUnmanagedCollectionField<long>(0);
            public static Message Parse(NativeBuffer buffer) => new Message(buffer);
        }
    }
}
