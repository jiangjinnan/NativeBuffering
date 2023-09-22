using NativeBuffering.NewDictionaries;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace NativeBuffering.Dictionaries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <remarks>Count(4)+Padding(4)+Key + KeyPadding + Value+ ValuePadding+...</remarks>
    internal unsafe readonly struct BufferedUnamanagedUnamanagedDictionartyEntry<TKey, TValue> : IReadOnlyBufferedObject<BufferedUnamanagedUnamanagedDictionartyEntry<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>
     where TKey : unmanaged, IEquatable<TKey>
     where TValue : unmanaged
    {        
        public static BufferedUnamanagedUnamanagedDictionartyEntry<TKey, TValue> DefaultValue { get; } = new(new NativeBuffer(new byte[4]));
        public BufferedUnamanagedUnamanagedDictionartyEntry(NativeBuffer buffer) => Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Unsafe.Read<int>(Buffer.Start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BufferedUnamanagedUnamanagedDictionartyEntry<TKey, TValue> Parse(NativeBuffer buffer) => new(buffer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new(this);
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()=> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public bool TryGetKV(TKey key, out UnmanagedUnmanagedPair<TKey, TValue> kvPair)
        {
            if (Count == 0)
            {
                kvPair = default;
                return false;
            }
            for (int index = 0; index < Count; index++)
            {
                var offset = IntPtr.Size + sizeof(UnmanangedKV<TKey, TValue>) * index;
                var kv = UnmanagedUnmanagedPair<TKey, TValue>.Parse(Buffer.CreateByOffset(offset));
                if (kv.Key.Equals(key))
                {
                    kvPair = kv;
                    return true;
                }
            }
            kvPair = default;
            return false;
        }

        internal struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly int _count;
            private readonly BufferedUnamanagedUnamanagedDictionartyEntry<TKey, TValue> _entry;
            private int _index = -1;

            public Enumerator(BufferedUnamanagedUnamanagedDictionartyEntry<TKey, TValue> entry)
            {
                _entry = entry;
                _count = entry.Count;
            }

            public readonly KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    var offset = IntPtr.Size + sizeof(UnmanangedKV<TKey, TValue>) * _index;
                    var kv = UnmanagedUnmanagedPair<TKey, TValue>.Parse(_entry.Buffer.CreateByOffset(offset));
                    return new KeyValuePair<TKey, TValue>(kv.Key, kv.Value);
                }
            }

            readonly object IEnumerator.Current => Current!;
            public readonly void Dispose() { }
            public bool MoveNext() => ++_index < _count;
            public void Reset() => _index = -1;
        }
    }
}
