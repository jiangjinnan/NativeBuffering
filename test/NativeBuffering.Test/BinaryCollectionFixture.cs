using NativeBuffering;
using NativeBuffering.Collections;

namespace BufferedMessaging.Test
{
    public class BinaryCollectionFixture
    {
        [Fact]
        public void GetValue()
        {
            var source = new Source(new List<byte[]> { new byte[] { 1, 1, 1 }, new byte[] { 2, 2, 2 }, new byte[] { 3, 3, 3 } });
            var buffer = new byte[source.CalculateSize()];
            var context = new BufferedObjectWriteContext(buffer);
            source.Write(context);
            var message = Message.Parse(new NativeBuffer(buffer, 0));
            Assert.Equal(3, message.Value.Count);

            Assert.True(message.Value[0].AsSpan().ToArray().All(it => it == 1));
            Assert.True(message.Value[1].AsSpan().ToArray().All(it => it == 2));
            Assert.True(message.Value[2].AsSpan().ToArray().All(it => it == 3));
        }

        public class Source : IBufferedObjectSource
        {
            public Source(List<byte[]> value)
            {
                Value = value;
            }

            public List<byte[]> Value { get; }
            public int CalculateSize() => Utilities.CalculateCollectionFieldSize(Value);

            public void Write(BufferedObjectWriteContext context)
            {
                using var scope = new BufferedObjectWriteContextScope(context);
                scope.WriteBinaryCollectionField(Value);
            }
        }

        public unsafe readonly struct Message : IReadOnlyBufferedObject<Message>
        {
            public Message(NativeBuffer buffer)=> Buffer = buffer;
            public NativeBuffer Buffer { get; }
            public ReadOnlyVariableLengthTypeList<BufferedBinary> Value => Buffer.ReadBufferedObjectCollectionField<BufferedBinary>(0);
            public static Message Parse(NativeBuffer buffer) => new Message(buffer);
        }
    }
}
