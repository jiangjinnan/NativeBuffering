using NativeBuffering.Collections;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NativeBuffering.Dictionaries
{
    public unsafe readonly struct ReadOnlyUnmanagedNonNullableUnmanagedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IReadOnlyBufferedObject<ReadOnlyUnmanagedNonNullableUnmanagedDictionary<TKey, TValue>>
         where TKey : unmanaged, IEquatable<TKey>
         where TValue : unmanaged
    {
        public static ReadOnlyUnmanagedNonNullableUnmanagedDictionary<TKey, TValue> DefaultValue { get; } = new(new NativeBuffer(new byte[4]));
        public ReadOnlyUnmanagedNonNullableUnmanagedDictionary(NativeBuffer buffer) => Buffer = buffer;
        public TValue this[TKey key] => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();
        public ref TValue AsRef(TKey key)
        {
            var kvs = GetDictionaryEntry(key.GetDictionaryEntryIndex(EntrySlotCount));
            if (kvs.TryGetKV(key, out var kv))
            {
                return ref kv.Value;
            }
            throw new KeyNotFoundException();
        }
        public NativeBuffer Buffer { get; }

        public IEnumerable<TKey> Keys
        {
            get
            {
                if (Count == 0)
                {
                    return Enumerable.Empty<TKey>();
                }
                var keys = new List<TKey>(Count);
                for (int index = 0; index < EntrySlotCount; index++)
                {
                    var kv = GetDictionaryEntry(index);
                    keys.AddRange(kv.Select(it => it.Key));
                }
                return keys;
            }
        }
        public IEnumerable<TValue> Values
        {
            get
            {
                if (Count == 0)
                {
                    return Enumerable.Empty<TValue>();
                }
                var values = new List<TValue>(Count);
                for (int index = 0; index < EntrySlotCount; index++)
                {
                    var kv = GetDictionaryEntry(index);
                    values.AddRange(kv.Select(it => it.Value));
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
        public static ReadOnlyUnmanagedNonNullableUnmanagedDictionary<TKey, TValue> Parse(NativeBuffer buffer) => new(buffer);
        public bool ContainsKey(TKey key) => TryGetValue(key, out _);

        public Enumerator GetEnumerator() => new(this);
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            var kvs = GetDictionaryEntry(key.GetDictionaryEntryIndex(EntrySlotCount));
            if (kvs.TryGetKV(key, out var kv))
            {
                value = kv.Value;
                return true;
            }
            value = default;
            return false;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        private BufferedUnamanagedUnamanagedDictionartyEntry<TKey, TValue> GetDictionaryEntry(int index)
        {
            var position = Unsafe.Read<int>(Buffer.GetPointerByOffset(sizeof(int) * (index + 2)));
            return position == -1 ? BufferedUnamanagedUnamanagedDictionartyEntry<TKey, TValue>.DefaultValue : BufferedUnamanagedUnamanagedDictionartyEntry<TKey, TValue>.Parse(Buffer.CreateByIndex(position));
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly ReadOnlyUnmanagedNonNullableUnmanagedDictionary<TKey, TValue> _dictionary;
            private KeyValuePair<TKey, TValue> _current;
            private int _entryIndex = 0;
            private BufferedUnamanagedUnamanagedDictionartyEntry<TKey, TValue>.Enumerator _entryEnerator;
            public Enumerator(ReadOnlyUnmanagedNonNullableUnmanagedDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                Reset();
            }
            public readonly KeyValuePair<TKey, TValue> Current => _current;
            readonly object IEnumerator.Current => Current;
            public readonly void Dispose() { }
            public bool MoveNext()
            {
                while (true)
                {
                    if (_entryEnerator.MoveNext())
                    {
                        _current = new KeyValuePair<TKey, TValue>(_entryEnerator.Current.Key, _entryEnerator.Current.Value);
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
    internal readonly record struct UnmanangedKV<TKey, TValue>(TKey Key, TValue Value) where TKey : unmanaged where TValue : unmanaged { }
}