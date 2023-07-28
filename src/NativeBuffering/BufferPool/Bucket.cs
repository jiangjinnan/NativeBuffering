using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace NativeBuffering
{
    internal sealed class Bucket
    {
        private readonly ConcurrentBag<byte[]> _pool = new();
        public void Add(byte[] array) => _pool.Add(array);
        public bool TryTake([MaybeNullWhen(false)]out byte[] array) => _pool.TryTake(out array);
    }
}
