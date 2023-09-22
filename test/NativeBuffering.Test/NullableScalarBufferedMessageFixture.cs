namespace NativeBuffering.Test
{
    public partial class NullableScalarBufferedMessageFixture
    {
        [Fact]
        public void GetValue()
        {
            var source = new Source();
            using var pooledMessage = source.AsBufferedMessage<SourceBufferedMessage>();
            var message = pooledMessage.BufferedMessage;

            Assert.Null(message.Primitive);
            Assert.Null(message.Unmanaged);
            Assert.True(message.String == string.Empty);
            Assert.True(message.Bytes.AsSpan().Length == 0);
            Assert.Null(message.Point);
        }

        public readonly record struct Foobar(int Foo, long Bar);
        [BufferedMessageSource]
        public partial class Source 
        {
            public int? Primitive { get; }
            public Foobar? Unmanaged { get; }
            public string? String { get; }
            public byte[]? Bytes { get; }     
            public Point? Point { get; }
        }

        [BufferedMessageSource]
        public partial struct Point
        {
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }
        }
    }
}
