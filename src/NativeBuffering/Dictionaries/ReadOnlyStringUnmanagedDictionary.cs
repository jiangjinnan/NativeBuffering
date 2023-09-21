using NativeBuffering.Collections;
using NativeBuffering.NewDictionaries;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NativeBuffering.Dictionaries
{
    public unsafe readonly struct ReadOnlyStringUnmanagedDictionary<TValue> : IReadOnlyDictionary<string, TValue>, IReadOnlyBufferedObject<ReadOnlyStringUnmanagedDictionary<TValue>>
         where TValue : unmanaged
    {
        public ReadOnlyStringUnmanagedDictionary(NativeBuffer buffer) => Buffer = buffer;
        public TValue this[string key] => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();

        public ref TValue AsRef(string key)
        {
            var kvs = GetDictionaryEntry(key.GetDictionaryEntryIndex(EntrySlotCount));
            if (ReadOnlyStringUnmanagedDictionary<TValue>.TryGetKV(kvs, key, out var kv))
            {
                return ref kv.Value;
            }
            throw new KeyNotFoundException();
        }
        public NativeBuffer Buffer { get; }
        public IEnumerable<string> Keys
        {
            get
            { 
                if(Count == 0)
                {
                    return Enumerable.Empty<string>();
                }   
                var keys = new List<string>(Count);
                for (int index = 0; index < EntrySlotCount; index++)
                {
                    var kv = GetDictionaryEntry(index);
                    keys.AddRange(kv.Select(it=>it.Key));
                }
                return keys;
            }
        }
        public IEnumerable<TValue> Values
        {
            get
            { 
                if(Count == 0)
                {
                    return Enumerable.Empty<TValue>();
                }
                var values = new List<TValue>(Count);
                for (int index = 0; index < EntrySlotCount; index++)
                {
                    var kv = GetDictionaryEntry(index);
                    values.AddRange(kv.Select(it=>it.Value));
                }
                return values;
            }
        }
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => DictionaryUtilities.GetCount(Buffer);
        }
        public int EntrySlotCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => DictionaryUtilities.GetEntrySlotCount(Buffer);
        }
        public static ReadOnlyStringUnmanagedDictionary<TValue> Parse(NativeBuffer buffer) => new(buffer);
        public bool ContainsKey(string key) => TryGetValue(key, out _);
       

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value)
        {
            var kvs = GetDictionaryEntry(key.GetDictionaryEntryIndex(EntrySlotCount));
            if (ReadOnlyStringUnmanagedDictionary<TValue>.TryGetKV(kvs, key, out var kv))
            {
                value = kv.Value;
                return true;
            }
            value = default;
            return false;
        }
        public Enumerator GetEnumerator() => new(this);
        IEnumerator<KeyValuePair<string, TValue>> IEnumerable<KeyValuePair<string, TValue>>.GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        private ReadOnlyVariableLengthTypeList<StringUnmanagedPair<TValue>> GetDictionaryEntry(int index)
        {
            var position = Unsafe.Read<int>(Buffer.GetPointerByOffset(sizeof(int) * (index + 2)));
            return position == -1 ? ReadOnlyVariableLengthTypeList<StringUnmanagedPair<TValue>>.Empty : ReadOnlyVariableLengthTypeList<StringUnmanagedPair<TValue>>.Parse(Buffer.CreateByIndex(position));
        }

        private static bool TryGetKV(ReadOnlyVariableLengthTypeList<StringUnmanagedPair<TValue>> entry, string key, [MaybeNullWhen(false)] out StringUnmanagedPair<TValue> kv)
        {
            for (var index = 0; index < entry.Count; index++)
            {
                var item = entry[index];
                if (item.Key.Equals(key))
                {
                    kv = item;
                    return true;
                }
            }

            kv = default;
            return false;
        }

        public struct Enumerator : IEnumerator<KeyValuePair<string, TValue>>
        {
            private readonly ReadOnlyStringUnmanagedDictionary<TValue> _dictionary;
            private KeyValuePair<string, TValue> _current;
            private int _entryIndex = 0;
            private ReadOnlyVariableLengthTypeList<StringUnmanagedPair<TValue>>.Enumerator _entryEnerator;

            public Enumerator(ReadOnlyStringUnmanagedDictionary<TValue> dictionary)
            {
                _dictionary = dictionary;
                Reset();
            }

            public readonly KeyValuePair<string, TValue> Current => _current;

            readonly object IEnumerator.Current => Current;

            public readonly void Dispose() { }

            public bool MoveNext()
            {
                while (true)
                {
                    if (_entryEnerator.MoveNext())
                    {
                        _current = new KeyValuePair<string, TValue>(_entryEnerator.Current.Key, _entryEnerator.Current.Value);
                        return true;
                    }
                    if (++_entryIndex >= _dictionary.EntrySlotCount)
                    {
                        return false;
                    }
                    _entryEnerator = _dictionary.GetDictionaryEntry(_entryIndex).GetEnumerator();
                }
            }

            public void Reset()
            {
                _entryIndex = 0;
                _entryEnerator = _dictionary.GetDictionaryEntry(_entryIndex).GetEnumerator();
            }
        }
    }
}