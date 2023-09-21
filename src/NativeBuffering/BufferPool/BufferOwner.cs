namespace NativeBuffering
{
    public sealed class BufferOwner: IDisposable
    {
        private readonly byte[] _bytes;
        private readonly Bucket? _bucket;
        private int _isReleased;

        internal BufferOwner(byte[] bytes, Bucket? bucket)
        {
            _bytes = bytes;
            _bucket = bucket;
        }

        public byte[] Bytes
        {
            get
            {
                if (_isReleased == 1)
                {
                    throw new ObjectDisposedException("The ByteArrayOwner has been released.");
                }
                return _bytes;
            }
        }


        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _isReleased, 1, 0) == 0)
            {
                _bucket?.Add(_bytes);
            }
        }
    }
}
