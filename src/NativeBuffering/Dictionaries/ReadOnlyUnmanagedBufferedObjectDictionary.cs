using NativeBuffering.Collections;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace NativeBuffering.Dictionaries
{
    public unsafe readonly struct ReadOnlyUnmanagedBufferedObjectDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>,IReadOnlyBufferedObject<ReadOnlyUnmanagedBufferedObjectDictionary<TKey, TValue>>
        where TKey : unmanaged, IComparable<TKey>
        where TValue : IReadOnlyBufferedObject<TValue>
    {
        private readonly ReadOnlyVariableLengthTypeList<UnmanagedBufferedObjectPair<TKey, TValue>> _list;
        public ReadOnlyUnmanagedBufferedObjectDictionary(NativeBuffer buffer) =>_list = new(buffer);
        public TValue this[TKey key] => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();
        public IEnumerable<TKey> Keys
        {
            get
            {
                var keys = new TKey[Count];
                for (int index = 0; index < Count; index++)
                {
                    var kv = _list[index];
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
                    var kv = _list[index];
                    values[index] = kv.Value;
                }
                return values;
            }
        }
        public int Count => _list.Count;
        public bool ContainsKey(TKey key)=>TryGetValue(key, out _);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()=> new Enumerator(this);
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)=> BinarySearch(0, Count, key, out value);
        IEnumerator IEnumerable.GetEnumerator()=> GetEnumerator();
        private bool BinarySearch(int low, int high, TKey key, out TValue value)
        {
            int index = (low + high) / 2;
            if (low > high)
            {
                value = default!;
                return false;
            }

            var kv = _list[index];
            var result = kv.Key.CompareTo(key);
            if (result == 0)
            {
                value = kv.Value;
                return true;
            }

            return result > 0 ? BinarySearch(low, index - 1, key, out value) : BinarySearch(index + 1, high, key, out value);
        }

        public static ReadOnlyUnmanagedBufferedObjectDictionary<TKey, TValue> Parse(NativeBuffer buffer)=> new(buffer);

        private readonly struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly IEnumerator<UnmanagedBufferedObjectPair<TKey, TValue>> _enumerator;
            public Enumerator(ReadOnlyUnmanagedBufferedObjectDictionary<TKey, TValue> dictionary)=>_enumerator = dictionary._list.GetEnumerator();
            public KeyValuePair<TKey, TValue> Current => _enumerator.Current.AsKeyValuePair(); 
            object IEnumerator.Current => Current;
            public readonly void Dispose() { }
            public readonly bool MoveNext() => _enumerator.MoveNext();
            public readonly void Reset() => _enumerator.Reset();
        }
    }
}
