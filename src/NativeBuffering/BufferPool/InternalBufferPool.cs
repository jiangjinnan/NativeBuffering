namespace NativeBuffering
{
    internal sealed class InternalBufferPool
    {
        private readonly Bucket[] buckets;
        private readonly int _maxRequestedLength;
        private readonly int _bucketCount;
        private readonly int _step;
        public static InternalBufferPool Create(int maxLength, int bucketCount)=> new(maxLength, bucketCount);

        public InternalBufferPool(int maxLength, int bucketCount)
        {
            _maxRequestedLength = maxLength;
            _bucketCount = bucketCount;
            _step = maxLength / (_bucketCount - 1);

            buckets = new Bucket[_bucketCount];
            for (int index = 0; index < _bucketCount; index++)
            {
                buckets[index] = new Bucket();
            }
        }

        public BufferOwner Rent(int minimumLength)
        {
            if (minimumLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumLength));
            }
            if (minimumLength > _maxRequestedLength)
            {
                return new BufferOwner( GC.AllocateUninitializedArray<byte>(minimumLength, pinned: true), null);
            }

            for (int index = IndexOf(minimumLength); index < _bucketCount; index++)
            {
                var bucket = buckets[index];
                if (bucket.TryTake(out var array))
                {
                    return new BufferOwner(array, bucket);
                }
            }

            return new BufferOwner( GC.AllocateUninitializedArray<byte>(minimumLength + _step, pinned: true), null);
        }

        private int IndexOf(int length) => length / _step;
    }
}
