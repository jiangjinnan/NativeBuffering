using System.Numerics;
using System.Runtime.CompilerServices;

namespace NativeBuffering
{
    internal sealed class InternalBufferPool
    {
        private readonly Bucket[] buckets;
        private readonly int _maxRequestedLength;
        public static InternalBufferPool Create(int maxLength) => new(maxLength);

        public InternalBufferPool(int maxLength)
        {
            var bucketCount = SelectBucketIndex(maxLength) + 1;
            buckets = new Bucket[bucketCount];
            _maxRequestedLength = GetMaxSizeForBucket(bucketCount - 1);
            for (int index = 0; index < bucketCount; index++)
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
                return new BufferOwner(GC.AllocateUninitializedArray<byte>(minimumLength, pinned: false), null);
            }

            var bucketIndex = SelectBucketIndex(minimumLength);
            for (int index = bucketIndex; index < buckets.Length; index++)
            {
                var bucket = buckets[index];
                if (bucket.TryTake(out var array))
                {
                    return new BufferOwner(array, bucket);
                }
            }

            return new BufferOwner(GC.AllocateUninitializedArray<byte>(GetMaxSizeForBucket(bucketIndex), pinned: true), buckets[bucketIndex]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetMaxSizeForBucket(int binIndex)=> 16 << binIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int SelectBucketIndex(int bufferSize)=> BitOperations.Log2((uint)(bufferSize - 1) | 0xFu) - 3;
    }
}
