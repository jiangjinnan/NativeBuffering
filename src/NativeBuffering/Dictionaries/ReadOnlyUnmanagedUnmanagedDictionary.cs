using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NativeBuffering.Dictionaries
{
    public unsafe readonly struct ReadOnlyUnmanagedUnmanagedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IReadOnlyBufferedObject<ReadOnlyUnmanagedUnmanagedDictionary<TKey, TValue>>
        where TKey : unmanaged, IComparable<TKey>
        where TValue : unmanaged
    {
        public ReadOnlyUnmanagedUnmanagedDictionary(NativeBuffer buffer)=> Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public TValue this[TKey key] => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();
        public readonly ref TValue AsRef(TKey index) 
        { 
            if(BinarySearch(0, Count, index, out var kv))
            {
                return ref kv.Value;
            }
            throw new KeyNotFoundException();
        }
        public IEnumerable<TKey> Keys
        {
            get
            {
                var keys = new TKey[Count];
                for (int index = 0; index < Count; index++)
                {
                    var kv =  Get(index);
                    keys[index] = kv.Key;
                }
                return keys;
            }
        }
        public IEnumerable<TValue> Values
        {
            get
            {
                var values = new TValue[Count];
                for (int index = 0; index < Count; index++)
                {
                    var kv = Get(index);
                    values[index] = kv.Value;
                }
                return values;
            }
        }
        public int Count => Unsafe.Read<int>(Buffer.Start);
        public static ReadOnlyUnmanagedUnmanagedDictionary<TKey, TValue> Parse(NativeBuffer buffer) => new(buffer);
        public bool ContainsKey(TKey key)=>ContainsKey(0, Count, key);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()=> new Enumerator(this);
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (BinarySearch(0, Count, key, out var kv))
            { 
                value = kv.Value;
                return true;
            }
            value = default!;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()=> GetEnumerator();
        private bool BinarySearch(int low, int high, TKey key, out UnmanagedUnmanagedPair<TKey, TValue> kv)
        {
            int index = (low + high) / 2;
            if (low > high)
            {
                kv = default!;
                return false;
            }

            kv = Get(index);
            var result = kv.Key.CompareTo(key);
            if (result == 0)
            {
                return true;
            }

            return result > 0 ? BinarySearch(low, index - 1, key, out kv) : BinarySearch(index + 1, high, key, out kv);
        }

        private bool ContainsKey(int low, int high, TKey key)
        {
            int index = (low + high) / 2;
            if (low > high)
            {
                return false;
            }

            var kv = Get(index);
            var result = kv.Key.CompareTo(key);
            if (result == 0)
            {
                return true;
            }

            return result > 0 ? ContainsKey(low, index - 1, key) : ContainsKey(index + 1, high, key);
        }

        private UnmanagedUnmanagedPair<TKey, TValue> Get(int index)
        { 
            var buffer = Buffer.CreateByOffset(sizeof(int) + index * UnmanagedUnmanagedPair<TKey, TValue>.CalculateSize());
            return UnmanagedUnmanagedPair<TKey, TValue>.Parse(buffer);
        }

        private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly ReadOnlyUnmanagedUnmanagedDictionary<TKey, TValue> _dictionary;
            private int _index;
            public Enumerator(ReadOnlyUnmanagedUnmanagedDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                _index = -1;
            }

            public bool MoveNext()=> ++_index < _dictionary.Count;
            public void Reset()=> _index = -1;
            public readonly KeyValuePair<TKey, TValue> Current => _dictionary.Get(_index).AsKeyValuePair();
            readonly object IEnumerator.Current => Current;
            public readonly void Dispose()
            {
            }
        }   
    }
}
