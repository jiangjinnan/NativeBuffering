namespace NativeBuffering.Test
{
    public partial class BinaryCollectionFixture
    {
        [Fact]
        public void GetValue()
        {
            var source = new Source(new List<byte[]?> { null, new byte[] { 2, 2, 2 }, Array.Empty<byte>()});
            var buffer = new byte[source.CalculateSize()];
            var context = new BufferedObjectWriteContext(buffer);
            source.Write(context);
            using var pooledMessage = source.AsBufferedMessage<SourceBufferedMessage>();
            var message = pooledMessage.BufferedMessage;
            Assert.Equal(3, message.Value.Count);

            Assert.True(message.Value[0]!.Length == 0);
            Assert.True(message.Value[0]!.AsSpan().Length == 0);
            Assert.True(message.Value[1]!.AsSpan().ToArray().All(it => it == 2));
            Assert.True(message.Value[2]!.Length == 0);
            Assert.True(message.Value[2]!.AsSpan().Length == 0);
        }

        [BufferedMessageSource]
        public partial class Source
        {
            public Source(List<byte[]?> value)
            {
                Value = value;
            }

            public IList<byte[]?> Value { get; }
        }
    }
}
