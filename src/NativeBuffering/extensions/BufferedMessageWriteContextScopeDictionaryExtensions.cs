namespace NativeBuffering
{
    public static class BufferedMessageWriteContextScopeDictionaryExtensions
    {
        public static void WriteUnmanagedUnmanagedDictionaryField<TKey, TValue>(this BufferedObjectWriteContextScope scope, IDictionary<TKey, TValue> dictionary)
           where TKey : unmanaged, IComparable<TKey>
           where TValue : unmanaged
        => scope.WriteField(dictionary, (c,v)=>c.WriteUnmanagedUnmanagedDictionary(v));

        public static void WriteUnmanagedStringDictionaryField<TKey>(this BufferedObjectWriteContextScope scope, IDictionary<TKey, string> dictionary)
           where TKey : unmanaged, IComparable<TKey>
        => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedStringDictionary(v));

        public static void WriteUnmanagedBinaryDictionaryField<TKey>(this BufferedObjectWriteContextScope scope, IDictionary<TKey, byte[]> dictionary)
          where TKey : unmanaged, IComparable<TKey>
        => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedBinaryDictionary(v));

        public static void WriteUnmanagedBinaryDictionaryField<TKey>(this BufferedObjectWriteContextScope scope, IDictionary<TKey, Memory<byte>> dictionary)
         where TKey : unmanaged, IComparable<TKey>
        => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedBinaryDictionary(v));

        public static void WriteUnmanagedBufferedObjectDictionaryField<TKey, TValue>(this BufferedObjectWriteContextScope scope, IDictionary<TKey, TValue> dictionary)
          where TKey : unmanaged, IComparable<TKey>
          where TValue : IBufferedObjectSource
       => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedBufferedObjectDictionary(v));

        public static void WriteStringUnmanagedDictionaryField<TValue>(this BufferedObjectWriteContextScope scope, IDictionary<string, TValue> dictionary)
          where TValue : unmanaged
       => scope.WriteField(dictionary, (c, v) => c.WriteStringUnmanagedDictionary(v));

        public static void WriteStringStringDictionaryField(this BufferedObjectWriteContextScope scope, IDictionary<string, string> dictionary)
        => scope.WriteField(dictionary, (c, v) => c.WriteStringStringDictionary(v));

        public static void WriteStringBinaryDictionaryField(this BufferedObjectWriteContextScope scope, IDictionary<string, byte[]> dictionary)
        => scope.WriteField(dictionary, (c, v) => c.WriteStringBinaryDictionary(v));

        public static void WriteStringBinaryDictionaryField(this BufferedObjectWriteContextScope scope, IDictionary<string, Memory<byte>> dictionary)
        => scope.WriteField(dictionary, (c, v) => c.WriteStringBinaryDictionary(v));

        public static void WriteStringBufferedObjectDictionaryField<TValue>(this BufferedObjectWriteContextScope scope, IDictionary<string, TValue> dictionary)
          where TValue : IBufferedObjectSource
       => scope.WriteField(dictionary, (c, v) => c.WriteStringBufferedObjectDictionary(v));
    }
}
