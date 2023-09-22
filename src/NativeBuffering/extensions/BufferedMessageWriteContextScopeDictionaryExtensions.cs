namespace NativeBuffering
{
    public static class BufferedMessageWriteContextScopeDictionaryExtensions
    {
        public static void WriteUnmanagedNonNullableUnmanagedDictionaryField<TKey, TValue>(this BufferedObjectWriteContextScope scope, IDictionary<TKey, TValue> dictionary)
           where TKey : unmanaged, IComparable<TKey>
           where TValue : unmanaged
        => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedNonNullableUnmanagedDictionary(v), IntPtr.Size);

        public static void WriteUnmanagedStringDictionaryField<TKey>(this BufferedObjectWriteContextScope scope, IDictionary<TKey, string> dictionary)
           where TKey : unmanaged, IComparable<TKey>
        => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedStringDictionary(v), IntPtr.Size);

        public static void WriteUnmanagedBinaryDictionaryField<TKey>(this BufferedObjectWriteContextScope scope, IDictionary<TKey, byte[]> dictionary)
          where TKey : unmanaged, IComparable<TKey>
        => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedBinaryDictionary(v), IntPtr.Size);

        public static void WriteUnmanagedBinaryDictionaryField<TKey>(this BufferedObjectWriteContextScope scope, IDictionary<TKey, Memory<byte>> dictionary)
         where TKey : unmanaged, IComparable<TKey>
        => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedBinaryDictionary(v), IntPtr.Size);

        public static void WriteUnmanagedBufferedObjectDictionaryField<TKey, TValue>(this BufferedObjectWriteContextScope scope, IDictionary<TKey, TValue> dictionary)
          where TKey : unmanaged, IComparable<TKey>
          where TValue : IBufferedObjectSource
       => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedBufferedObjectDictionary(v), IntPtr.Size);

        public static void WriteStringNonNullableUnmanagedDictionaryField<TValue>(this BufferedObjectWriteContextScope scope, IDictionary<string, TValue> dictionary)
          where TValue : unmanaged
       => scope.WriteField(dictionary, (c, v) => c.WriteStringNonNullableUnmanagedDictionary(v), IntPtr.Size);

        public static void WriteStringStringDictionaryField(this BufferedObjectWriteContextScope scope, IDictionary<string, string> dictionary)
        => scope.WriteField(dictionary, (c, v) => c.WriteStringStringDictionary(v), IntPtr.Size);

        public static void WriteStringBinaryDictionaryField(this BufferedObjectWriteContextScope scope, IDictionary<string, byte[]> dictionary)
        => scope.WriteField(dictionary, (c, v) => c.WriteStringBinaryDictionary(v), IntPtr.Size);

        public static void WriteStringBinaryDictionaryField(this BufferedObjectWriteContextScope scope, IDictionary<string, Memory<byte>> dictionary)
        => scope.WriteField(dictionary, (c, v) => c.WriteStringBinaryDictionary(v), IntPtr.Size);

        public static void WriteStringBufferedObjectDictionaryField<TValue>(this BufferedObjectWriteContextScope scope, IDictionary<string, TValue> dictionary)
          where TValue : IBufferedObjectSource
       => scope.WriteField(dictionary, (c, v) => c.WriteStringBufferedObjectDictionary(v), IntPtr.Size);
    }
}
