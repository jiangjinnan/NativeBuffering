namespace NativeBuffering.Test
{
    public class ScalarBufferedMessageFixture
    {
        [Fact]
        public void GetValue()
        {
            var source = new Source(1, new Foobar(3, 4), "foobar", new byte[] { 1, 2, 3 });
            var buffer = new byte[source.CalculateSize()];
            var context = new BufferedObjectWriteContext(buffer);
            source.Write(context);
            var message = Message.Parse(new NativeBuffer(buffer, 0));

            Assert.Equal(1, message.Primitive);
            Assert.Equal(3, message.Unmanaged.Foo);
            Assert.Equal(4, message.Unmanaged.Bar);
            Assert.Equal("foobar", message.String);

            var bytes = message.Bytes.AsSpan();
            Assert.Equal(3, bytes.Length);
            Assert.Equal(1, bytes[0]);
            Assert.Equal(2, bytes[1]);
            Assert.Equal(3, bytes[2]);
        }

        public readonly record struct Foobar(int Foo, long Bar);
        public class Source : IBufferedObjectSource
        {
            public Source(int primitive, Foobar foobar, string @string, byte[] bytes)
            {
                Primitive = primitive;
                Unmanaged = foobar;
                String = @string;
                Bytes = bytes;
            }

            public int Primitive { get; }
            public Foobar Unmanaged { get; }
            public string String { get; }
            public byte[] Bytes { get; }
            public int CalculateSize()
            {
                var size = 0;
                size += Utilities.CalculateUnmanagedFieldSize(Primitive);
                size += Utilities.CalculateUnmanagedFieldSize(Unmanaged);
                size += Utilities.CalculateStringFieldSize(String);
                size += Utilities.CalculateBinaryFieldSize(Bytes);

                return size;
            }

            public void Write(BufferedObjectWriteContext context)
            {
                using var scope = new BufferedObjectWriteContextScope(context);
                scope.WriteUnmanagedField(Primitive);
                scope.WriteUnmanagedField(Unmanaged);
                scope.WriteStringField(String);
                scope.WriteBinaryField(Bytes);
            }
        }

        public unsafe readonly struct Message : IReadOnlyBufferedObject<Message>
        {
            public Message(NativeBuffer buffer) => Buffer = buffer;
            public NativeBuffer Buffer { get; }

            public int Primitive => Buffer.ReadUnmanagedField<int>(0);
            public ref Foobar Unmanaged => ref Buffer.ReadUnmanagedFieldAsRef<Foobar>(1);
            public string String => Buffer.ReadBufferedObjectField<BufferedString>(2);
            public BufferedBinary Bytes => Buffer.ReadBufferedObjectField<BufferedBinary>(3);
            public static Message Parse(NativeBuffer buffer) => new(buffer);
        }
    }
}
