using NativeBuffering.Dictionaries;

namespace NativeBuffering
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Materialize<TKey, TValue>(this ReadOnlyUnmanagedUnmanagedDictionary<TKey, TValue> bufferedDictionary)
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : unmanaged
            => bufferedDictionary.ToDictionary(kv => kv.Key, kv => kv.Value);

        public static Dictionary<TKey, TValue> Materialize<TKey, TValue>(this ReadOnlyUnmanagedBufferedObjectDictionary<TKey, TValue> bufferedDictionary)
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : IReadOnlyBufferedObject<TValue>
            => bufferedDictionary.ToDictionary(kv => kv.Key, kv => kv.Value);

        public static Dictionary<string, TValue> Materialize<TValue>(this ReadOnlyStringUnmanagedDictionary<TValue> bufferedDictionary)
        where TValue : unmanaged
           => bufferedDictionary.ToDictionary(kv => kv.Key, kv => kv.Value);

        public static Dictionary<string, TValue> Materialize<TValue>(this ReadOnlyStringBufferedObjectDictionary<TValue> bufferedDictionary)
         where TValue : IReadOnlyBufferedObject<TValue>
           => bufferedDictionary.ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}
