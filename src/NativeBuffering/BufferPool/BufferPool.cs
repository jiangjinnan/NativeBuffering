namespace NativeBuffering
{
    public static class BufferPool
    {
        private static int _maxRequestedLength = 1024 * 1024;
        private static InternalBufferPool? _pool;

        public static void Configure(int maxRequestedLength)
        {
            if (_pool is not null)
            {
                throw new InvalidOperationException("BufferPool can only be configure before the 1st usage.");
            }
            _maxRequestedLength = maxRequestedLength;
        }

        public static BufferOwner Rent(int minimumLength)
        {
            if (_pool is null)
            {
                Interlocked.CompareExchange(ref _pool, new InternalBufferPool(_maxRequestedLength), null);
            }
            return _pool.Rent(minimumLength);
        }
    }
}
