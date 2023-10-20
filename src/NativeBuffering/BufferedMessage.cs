namespace NativeBuffering
{
    public static class BufferedMessage
    {
        public static T Create<T>(ref BufferOwner byteArrayOwner, int offset = 0) where T : IReadOnlyBufferedObject<T>
        {
            return T.Parse(new NativeBuffer(byteArrayOwner.Bytes, offset));
        }

        public static PooledBufferedMessage<T> AsBufferedMessage<T>(this IBufferedObjectSource source) where T : IReadOnlyBufferedObject<T>
        {
            var size = (source ?? throw new ArgumentNullException(nameof(source))).CalculateSize();
            var owner = BufferPool.Rent(size);
            var context = BufferedObjectWriteContext.Create(owner.Bytes);
            source.Write(context);
            return new PooledBufferedMessage<T>(owner, Create<T>(ref owner));
        }
    }

    public sealed class PooledBufferedMessage<T>(BufferOwner bufferOwner, T bufferedMessage):IDisposable where T : IReadOnlyBufferedObject<T>
    {
        private readonly BufferOwner _bufferOwner = bufferOwner;
        private readonly T _bufferedMessage = bufferedMessage;

        public T BufferedMessage => _bufferedMessage;
        public void Dispose()=> _bufferOwner.Dispose();
    }
}
