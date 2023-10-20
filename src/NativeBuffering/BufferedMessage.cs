namespace NativeBuffering
{
    public static class BufferedMessage
    {
        private static T Create<T>(ref BufferOwner byteArrayOwner, int offset = 0) where T : IReadOnlyBufferedObject<T>
        => T.Parse(new NativeBuffer(byteArrayOwner.Bytes, offset));

        public static PooledBufferedMessage<T> AsBufferedMessage<T>(this IBufferedObjectSource source) where T : IReadOnlyBufferedObject<T>
        {
            var size = (source ?? throw new ArgumentNullException(nameof(source))).CalculateSize();
            var owner = BufferPool.Rent(size);
            var context = BufferedObjectWriteContext.Create(owner.Bytes);
            source.Write(context);
            return new PooledBufferedMessage<T>(owner, Create<T>(ref owner));
        }

        public static PooledBufferedMessage<T> Load<T>(Stream stream) where T : IReadOnlyBufferedObject<T>
        {
            var size = (int)stream.Length;
            var owner = BufferPool.Rent(size);
            stream.Read(owner.Bytes, 0, size);
            return new PooledBufferedMessage<T>(owner, Create<T>(ref owner));
        }

        public static async Task<PooledBufferedMessage<T>> LoadAsync<T>(Stream stream, bool closeStream) where T : IReadOnlyBufferedObject<T>
        {
            var size = (int)stream.Length;
            var owner = BufferPool.Rent(size);
            await stream.ReadAsync(owner.Bytes.AsMemory(0, size));
            if (closeStream) stream.Close();
            return new PooledBufferedMessage<T>(owner, Create<T>(ref owner));
        }

        public static Task<PooledBufferedMessage<T>> LoadAsync<T>(string filePath) where T : IReadOnlyBufferedObject<T>
        => LoadAsync<T>(new FileStream(filePath, FileMode.Open, FileAccess.Read), true);
    }

    public sealed class PooledBufferedMessage<T>(BufferOwner bufferOwner, T bufferedMessage) : IDisposable where T : IReadOnlyBufferedObject<T>
    {
        private readonly BufferOwner _bufferOwner = bufferOwner;
        private readonly T _bufferedMessage = bufferedMessage;

        public T BufferedMessage => _bufferedMessage;
        public void Dispose() => _bufferOwner.Dispose();
    }
}
