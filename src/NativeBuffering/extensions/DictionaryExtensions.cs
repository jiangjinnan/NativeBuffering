using NativeBuffering.Dictionaries;

namespace NativeBuffering
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Materialize<TKey, TValue>(this ReadOnlyUnmanagedNonNullableUnmanagedDictionary<TKey, TValue> bufferedDictionary)
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : unmanaged
            => bufferedDictionary.ToDictionary(kv => kv.Key, kv => kv.Value);

        public static Dictionary<TKey, TValue> Materialize<TKey, TValue>(this ReadOnlyUnmanagedNonNullableBufferedObjectDictionary<TKey, TValue> bufferedDictionary)
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : struct, IReadOnlyBufferedObject<TValue>
            => bufferedDictionary.ToDictionary(kv => kv.Key, kv => kv.Value);

        public static Dictionary<string, TValue> Materialize<TValue>(this ReadOnlyStringNonNullableUnmanagedDictionary<TValue> bufferedDictionary)
        where TValue : unmanaged
           => bufferedDictionary.ToDictionary(kv => kv.Key, kv => kv.Value);

        public static Dictionary<string, TValue> Materialize<TValue>(this ReadOnlyStringNonNullableBufferedObjectDictionary<TValue> bufferedDictionary)
         where TValue :struct, IReadOnlyBufferedObject<TValue>
           => bufferedDictionary.ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}
