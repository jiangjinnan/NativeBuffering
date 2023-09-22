using NativeBuffering.Dictionaries;

namespace NativeBuffering.Test
{
    public partial class StringBufferedNullableObjectDictionaryFixture_Structure
    {
        [Fact]
        public void GetValues()
        { 
            var source = new Source(new Dictionary<string, Foobar?> { { "1", new Foobar(1,1) } ,{ "2", null}, { "3", new Foobar(3,3) }});
            using var pooledMessage = source.AsBufferedMessage<SourceBufferedMessage>();
            var message = pooledMessage.BufferedMessage;
            Assert.Equal(3, message.Value.Count);
            Assert.Equal(1, message.Value["1"]!.Value.Foo);
            Assert.Equal(3, message.Value["3"]!.Value.Foo);
            Assert.Equal(1, message.Value["1"]!.Value.Bar);
            Assert.Equal(3, message.Value["3"]!.Value.Bar);
            Assert.Null(message.Value["2"]);
        }

        [BufferedMessageSource]
        public partial struct Source
        {
            public Source(Dictionary<string, Foobar?> value)
            {
                Value = value;
            }

            public Dictionary<string, Foobar?> Value { get; }            
        }

        [BufferedMessageSource]
        public partial struct Foobar 
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
