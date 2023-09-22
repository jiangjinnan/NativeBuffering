namespace NativeBuffering.Test
{
    public partial class NonNullableScalarBufferedMessageFixture
    {
        [Fact]
        public void GetValue()
        {
            var source = new Source(1, new Foobar(3, 4), "foobar", new byte[] { 1, 2, 3 }, new Point(1,3));
            using var pooledMessage = source.AsBufferedMessage<SourceBufferedMessage>();
            var message = pooledMessage.BufferedMessage;

            Assert.Equal(1, message.Primitive);
            Assert.Equal(3, message.Unmanaged.Foo);
            Assert.Equal(4, message.Unmanaged.Bar);
            Assert.Equal("foobar", message.String);

            var bytes = message.Bytes.AsSpan();
            Assert.Equal(3, bytes.Length);
            Assert.Equal(1, bytes[0]);
            Assert.Equal(2, bytes[1]);
            Assert.Equal(3, bytes[2]);

            var point = message.Point!.Value;
            Assert.Equal(1, point.X);
            Assert.Equal(3, point.Y);
        }

        public readonly record struct Foobar(int Foo, long Bar);

        [BufferedMessageSource]
        public partial class Source 
        {
            public Source(int primitive, Foobar foobar, string @string, byte[] bytes, Point point)
            {
                Primitive = primitive;
                Unmanaged = foobar;
                String = @string;
                Bytes = bytes;
                Point = point;
            }

            public int Primitive { get; }
            public Foobar Unmanaged { get; }
            public string String { get; }
            public byte[] Bytes { get; }     
            public Point Point { get; }
        }

        [BufferedMessageSource]
        public partial class Point
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
