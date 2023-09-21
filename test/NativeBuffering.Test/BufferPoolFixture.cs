namespace NativeBuffering.Test
{
    public class BufferPoolFixture
    {
        [Fact]
        public void Rent()
        { 
            var owner = BufferPool.Rent(100);
            Assert.True(owner.Bytes.Length >= 100);

            var bytes1 = owner.Bytes;
            owner.Dispose();

            var bytes2 = BufferPool.Rent(101).Bytes;
            Assert.Same(bytes1, bytes2);

            var bytes3 = BufferPool.Rent(101).Bytes;
            Assert.NotSame(bytes1, bytes3);
        }
    }
}
