using NativeBuffering;
using NativeBuffering.Collections;
using System.Runtime.CompilerServices;

namespace NativeBuffering.Test
{
    public partial class NullableUnmanagedCollectionFixture
    {
        [Fact]
        public void GetValue()
        {
            var source = new Source(new long?[] { 1, null, 3 });
            using var pooledMessage = source.AsBufferedMessage<SourceBufferedMessage>();
            var message = pooledMessage.BufferedMessage;
            Assert.Equal(3, message.Value.Count);
            Assert.Equal(1, message.Value[0]);
            Assert.Null(message.Value[1]);
            Assert.Equal(3, message.Value[2]);

            Assert.Contains(message.Value, it => it == 1);
            Assert.Contains(message.Value, it => it is null);
            Assert.Contains(message.Value, it => it == 3);
        }

        [BufferedMessageSource]
        public partial class Source 
        {
            public Source(long?[] value)
            {
                Value = value;
            }

            public long?[] Value { get; }
        }
    }
}
