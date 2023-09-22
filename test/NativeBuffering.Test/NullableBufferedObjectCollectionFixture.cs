namespace NativeBuffering.Test
{
    public partial class NullableBufferedObjectCollectionFixture
    {
        [Fact]
        public void GetValue()
        {
            var source = new Source(new Foobar1?[] { new Foobar1(1, 1), null, new Foobar1(3, 3) }
            , new Foobar2?[] { new Foobar2(1, 1), null, new Foobar2(3, 3) });
            using (var pooledMessage = source.AsBufferedMessage<SourceBufferedMessage>())
            {
                var message = pooledMessage.BufferedMessage;

                Assert.Equal(3, message.Value1.Count);
                Assert.Equal(3, message.Value1.Count());

                Assert.Equal(1, message.Value1[0]!.Value.Foo);
                Assert.Equal(3, message.Value1[2]!.Value.Foo);
                Assert.Equal(1, message.Value1[0]!.Value.Bar);
                Assert.Equal(3, message.Value1[2]!.Value.Bar);

                Assert.Null(message.Value1[1]);

                var index = 0;
                for (; index < 3; index++)
                {
                    if (index == 1)
                    {
                        Assert.Null(message.Value2[index]);
                    }
                    else
                    {
                        Assert.Equal(index + 1, message.Value1[index]!.Value.Foo);
                        Assert.Equal(index + 1, message.Value1[index]!.Value.Bar);
                    }
                }

                index = 0;
                foreach (var item in message.Value1)
                {
                    index++;
                    if (index == 2)
                    {
                        Assert.Null(item);
                    }
                    else
                    {
                        Assert.Equal(index, item!.Value.Foo);
                        Assert.Equal(index, item!.Value.Bar);
                    }
                }

                Assert.Equal(3, message.Value2.Count);
                Assert.Equal(3, message.Value2.Count());
                Assert.Equal(1, message.Value2[0]!.Value.Foo);
                Assert.Equal(3, message.Value2[2]!.Value.Foo);
                Assert.Equal(1, message.Value2[0]!.Value.Bar);
                Assert.Equal(3, message.Value2[2]!.Value.Bar);

                Assert.Null(message.Value2[1]);

                index = 0;
                for (; index < 3; index++)
                {
                    if (index == 1)
                    {
                        Assert.Null(message.Value2[index]);
                    }
                    else
                    {
                        Assert.Equal(index + 1, message.Value2[index]!.Value.Foo);
                        Assert.Equal(index + 1, message.Value2[index]!.Value.Bar);
                    }
                }

                index = 0;
                foreach (var item in message.Value2)
                {
                    index++;
                    if (index == 2)
                    {
                        Assert.Null(item);
                    }
                    else
                    {
                        Assert.Equal(index, item!.Value.Foo);
                        Assert.Equal(index, item!.Value.Bar);
                    }
                }
            }
        }


        [BufferedMessageSource]
        public partial class Source
        {
            public Source(Foobar1?[] value1, Foobar2?[] value2)
            {
                Value1 = value1;
                Value2 = value2;
            }

            public Foobar1?[] Value1 { get; }
            public Foobar2?[] Value2 { get; }
        }

        [BufferedMessageSource]
        public partial struct Foobar1
        {
            public Foobar1(int foo, long bar)
            {
                Foo = foo;
                Bar = bar;
            }

            public int Foo { get; }
            public long Bar { get; }
        }

        [BufferedMessageSource]
        public partial class Foobar2
        {
            public Foobar2(int foo, long bar)
            {
                Foo = foo;
                Bar = bar;
            }

            public int Foo { get; }
            public long Bar { get; }
        }
    }
}
