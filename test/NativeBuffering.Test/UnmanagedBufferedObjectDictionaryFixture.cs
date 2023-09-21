using NativeBuffering.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeBuffering.Test
{
    public partial class UnmanagedBufferedObjectDictionaryFixture
    {
        [Fact]
        public void GetValues()
        { 
            var source = new Source(new Dictionary<long, Foobar> { { 1, new Foobar(1,1) }, { 2, new Foobar(2,2) }, { 3, new Foobar(3,3)} });
            using var pooledMessage = source.AsBufferedMessage<SourceBufferedMessage>();
            var message = pooledMessage.BufferedMessage;
            Assert.Equal(3, message.Value.Count);
            Assert.Equal(1, message.Value[1].Foo);
            Assert.Equal(2, message.Value[2].Foo);
            Assert.Equal(3, message.Value[3].Foo);
            Assert.Equal(1, message.Value[1].Bar);
            Assert.Equal(2, message.Value[2].Bar);
            Assert.Equal(3, message.Value[3].Bar);
        }

        [BufferedMessageSource]
        public partial class Source 
        {
            public Source(Dictionary<long, Foobar> value)
            {
                Value = value;
            }

            public Dictionary<long, Foobar> Value { get; }
        }

        [BufferedMessageSource]
        public partial class Foobar 
        {
            public Foobar(int foo, long bar)
            {
                Foo = foo;
                Bar = bar;
            }

            public int Foo { get; }
            public long Bar { get; }
        }
    }
}
