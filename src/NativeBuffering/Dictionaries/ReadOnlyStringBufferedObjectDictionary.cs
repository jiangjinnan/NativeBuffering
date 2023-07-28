using NativeBuffering.Collections;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace NativeBuffering.Dictionaries
{
    public unsafe readonly struct ReadOnlyStringBufferedObjectDictionary<TValue> : IReadOnlyDictionary<string, TValue>, IReadOnlyBufferedObject<ReadOnlyStringBufferedObjectDictionary<TValue>>
        where TValue : IReadOnlyBufferedObject<TValue>
    {
        private readonly ReadOnlyVariableLengthTypeList<StringBufferedObjectPair<TValue>> _list;
        public ReadOnlyStringBufferedObjectDictionary(NativeBuffer buffer) =>_list = new(buffer);
        public TValue this[string key] => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();
        public IEnumerable<string> Keys
        {
            get
            {
                var keys = new string[Count];
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
        public bool ContainsKey(string key)=>ContainsKey(0, Count, key);
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()=> new Enumerator(this);
        public bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value)
        { 
            if(BinarySearch(0, Count, key, out var kv))
            {
                value = kv.Value;
                return true;
            }
            value = default!;
            return false;
        }
        IEnumerator IEnumerable.GetEnumerator()=> GetEnumerator();

        private bool BinarySearch(int low, int high, string key, out StringBufferedObjectPair<TValue> kv)
        {
            int index = (low + high) / 2;
            if (low > high)
            {
                kv = default!;
                return false;
            }

            kv = _list[index];
            var result = kv.Key.CompareTo(key);
            if (result == 0)
            {
                return true;
            }

            return result > 0 ? BinarySearch(low, index - 1, key, out kv) : BinarySearch(index + 1, high, key, out kv);
        }

        private bool ContainsKey(int low, int high, string key)
        {
            int index = (low + high) / 2;
            if (low > high)
            {
                return false;
            }

            var kv = _list[index];
            var result = kv.Key.CompareTo(key);
            if (result == 0)
            {
                return true;
            }

            return result > 0 ? ContainsKey(low, index - 1, key) : ContainsKey(index + 1, high, key);
        }

        public static ReadOnlyStringBufferedObjectDictionary<TValue> Parse(NativeBuffer buffer)=> new(buffer);

        private readonly struct Enumerator : IEnumerator<KeyValuePair<string, TValue>>
        {
            private readonly IEnumerator<StringBufferedObjectPair<TValue>> _enumerator;
            public Enumerator(ReadOnlyStringBufferedObjectDictionary< TValue> dictionary)=>_enumerator = dictionary._list.GetEnumerator();
            public KeyValuePair<string, TValue> Current => _enumerator.Current.AsKeyValuePair(); 
            object IEnumerator.Current => Current;
            public readonly void Dispose() { }
            public readonly bool MoveNext() => _enumerator.MoveNext();
            public readonly void Reset() => _enumerator.Reset();
        }
    }
}
