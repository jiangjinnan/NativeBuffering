namespace NativeBuffering.Test
{
    public partial class StringCollectionFixture
    {
        [Fact]
        public void GetValue()
        {
            var source = new Source(new string[] { "1", "2", "3" });
            using var pooledMessage = source.AsBufferedMessage<SourceBufferedMessage>();
            var message = pooledMessage.BufferedMessage;
            Assert.Equal(3, message.Value.Count);
            Assert.Equal("1", message.Value[0]);
            Assert.Equal("2", message.Value[1]);
            Assert.Equal("3", message.Value[2]);

            Assert.Contains(message.Value, it => it == "1");
            Assert.Contains(message.Value, it => it == "2");
            Assert.Contains(message.Value, it => it == "3");
        }

        [BufferedMessageSource]
        public partial class Source 
        {
            public Source(string[] value)
            {
                Value = value;
            }

            public string[] Value { get; }
        }
    }
}
