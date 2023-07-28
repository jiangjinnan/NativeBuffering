using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace NativeBuffering
{
    public static class BufferPool
    {
        private static int _maxRequestedLength = 1024 * 1024;
        private static int _bucketCount = 128;
        private static InternalBufferPool? _pool;

        public static void Configure(int maxRequestedLength, int bucketCount)
        {
            if (_pool is not null)
            {
                throw new InvalidOperationException("BufferPool can only be configure before the 1st usage.");
            }
            _maxRequestedLength = maxRequestedLength;
            _bucketCount = bucketCount;
        }

        public static BufferOwner Rent(int minimumLength)
        {
            if (_pool is null)
            {
                Interlocked.CompareExchange(ref _pool, new InternalBufferPool(_maxRequestedLength, _bucketCount), null);
            }
            return _pool.Rent(minimumLength);
        }
    }
}
