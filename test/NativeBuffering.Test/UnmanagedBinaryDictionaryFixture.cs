using NativeBuffering.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeBuffering.Test
{
    public partial class UnmanagedBinaryDictionaryFixture
    {
        [Fact]
        public void GetValues()
        { 
            var source = new Source(new Dictionary<long, byte[]?> { { 1, new byte[] { 1, 1, 1 } }, { 2, null }, { 3, new byte[0] } });
            using var pooledMessage = source.AsBufferedMessage<SourceBufferedMessage>();
            var message = pooledMessage.BufferedMessage;
            Assert.Equal(3, message.Value.Count);
            Assert.True(message.Value[1].AsSpan().SequenceEqual(new byte[] { 1,1,1 }));
            Assert.True(message.Value[2].AsSpan().Length == 0);
            Assert.True(message.Value[3].AsSpan().Length == 0);
        }

        [BufferedMessageSource]
        public partial class Source 
        {
            public Source(Dictionary<long, byte[]?> value)
            {
                Value = value;
            }

            public Dictionary<long, byte[]?> Value { get; }
        }
    }
}
