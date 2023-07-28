using NativeBuffering.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeBuffering.Test
{
    public class UnmanagedBinaryDictionaryFixture
    {
        [Fact]
        public void GetValues()
        { 
            var source = new Source(new Dictionary<long, byte[]> { { 1, new byte[] { 1,1,1} }, { 2, new byte[] { 2,2,2} }, { 3, new byte[] { 3,3,3} } });
            var buffer = new byte[source.CalculateSize()];
            var context = new BufferedObjectWriteContext(buffer);
            source.Write(context);
            var message = Message.Parse(new NativeBuffer(buffer, 0));
            Assert.Equal(3, message.Value.Count);
            Assert.True(message.Value[1].AsSpan().SequenceEqual(new byte[] { 1,1,1 }));
            Assert.True(message.Value[2].AsSpan().SequenceEqual(new byte[] { 2,2,2 }));
            Assert.True(message.Value[3].AsSpan().SequenceEqual(new byte[] { 3,3,3 }));
        }

        public class Source : IBufferedObjectSource
        {
            public Source(Dictionary<long, byte[]> value)
            {
                Value = value;
            }

            public Dictionary<long, byte[]> Value { get; }
            public int CalculateSize() => Utilities.CalculateDictionaryFieldSize(Value);

            public void Write(BufferedObjectWriteContext context)
            {
                using var scope = new BufferedObjectWriteContextScope(context);
                scope.WriteUnmanagedBinaryDictionaryField(Value);
            }
        }

        public class Message: IReadOnlyBufferedObject<Message>
        {
            public Message(NativeBuffer buffer) => Buffer = buffer;
            public NativeBuffer Buffer { get; }
            public ReadOnlyUnmanagedBufferedObjectDictionary<long, BufferedBinary> Value => Buffer.ReadUnmanagedBufferedObjectDictionaryField<long, BufferedBinary>(0);
            public static Message Parse(NativeBuffer buffer) => new Message(buffer);
        }
    }
}
