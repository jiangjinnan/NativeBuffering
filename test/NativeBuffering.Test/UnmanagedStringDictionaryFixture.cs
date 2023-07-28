using NativeBuffering.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeBuffering.Test
{
    public class UnmanagedStringDictionaryFixture
    {
        [Fact]
        public void GetValues()
        { 
            var source = new Source(new Dictionary<long, string> { { 1, "foo" }, { 2, "bar" }, { 3, "baz" } });
            var buffer = new byte[source.CalculateSize()];
            var context = new BufferedObjectWriteContext(buffer);
            source.Write(context);
            var message = Message.Parse(new NativeBuffer(buffer, 0));
            Assert.Equal(3, message.Value.Count);
            Assert.Equal("foo", message.Value[1]);
            Assert.Equal("bar", message.Value[2]);
            Assert.Equal("baz", message.Value[3]);
        }

        public class Source : IBufferedObjectSource
        {
            public Source(Dictionary<long, string> value)
            {
                Value = value;
            }

            public Dictionary<long, string> Value { get; }
            public int CalculateSize() => Utilities.CalculateDictionaryFieldSize(Value);

            public void Write(BufferedObjectWriteContext context)
            {
                using var scope = new BufferedObjectWriteContextScope(context);
                scope.WriteUnmanagedStringDictionaryField(Value);
            }
        }

        public class Message: IReadOnlyBufferedObject<Message>
        {
            public Message(NativeBuffer buffer) => Buffer = buffer;
            public NativeBuffer Buffer { get; }
            public ReadOnlyUnmanagedBufferedObjectDictionary<long, BufferedString> Value => Buffer.ReadUnmanagedBufferedObjectDictionaryField<long, BufferedString>(0);
            public static Message Parse(NativeBuffer buffer) => new Message(buffer);
        }
    }
}
