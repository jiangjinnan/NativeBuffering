namespace NativeBuffering.Test
{
    public partial class BufferedObjectCollectionFixture
    {
        [Fact]
        public void GetValue()
        {
            var source = new Source(new Foobar[] { new Foobar(1, 1), new Foobar(2, 2), new Foobar(3, 3) });            
            using (var pooledMessage = source.AsBufferedMessage<SourceBufferedMessage>())
            {
                var message = pooledMessage.BufferedMessage;

                Assert.Equal(3, message.Value.Count);
                Assert.Equal(3, message.Value.Count());
                Assert.Equal(1, message.Value[0].Foo);
                Assert.Equal(2, message.Value[1].Foo);
                Assert.Equal(3, message.Value[2].Foo);
                Assert.Equal(1, message.Value[0].Bar);
                Assert.Equal(2, message.Value[1].Bar);
                Assert.Equal(3, message.Value[2].Bar);

                var index = 0;
                for (; index < 3; index++)
                {
                    Assert.Equal(index + 1, message.Value[index].Foo);
                    Assert.Equal(index + 1, message.Value[index].Bar);
                }

                index = 0;
                foreach (var item in message.Value)
                {
                    index++;
                    Assert.Equal(index, item.Foo);
                    Assert.Equal(index, item.Bar);
                }
            }
        }


        [BufferedMessageSource]
        public partial class Source
        {
            public Source(Foobar[] value)
            {
                Value = value;
            }

            public Foobar[] Value { get; }
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
