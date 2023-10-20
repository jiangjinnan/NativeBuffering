namespace NativeBuffering
{
    public static class BufferedMessageWriteContextScopeDictionaryExtensions
    {
        #region IDictionary
        public static void WriteUnmanagedNonNullableUnmanagedDictionaryField<TKey, TValue>(this BufferedObjectWriteContextScope scope, IDictionary<TKey, TValue> dictionary)
           where TKey : unmanaged, IComparable<TKey>
           where TValue : unmanaged
        => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedNonNullableUnmanagedDictionary(v), IntPtr.Size);      

        public static void WriteUnmanagedNullableUnmanagedDictionaryField<TKey, TValue>(this BufferedObjectWriteContextScope scope, IDictionary<TKey, TValue?> dictionary)
           where TKey : unmanaged, IComparable<TKey>
           where TValue : unmanaged
        => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedNullableUnmanagedDictionary(v), IntPtr.Size);

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

        public static void WriteStringNullableUnmanagedDictionaryField<TValue>(this BufferedObjectWriteContextScope scope, IDictionary<string, TValue?> dictionary)
         where TValue : unmanaged
      => scope.WriteField(dictionary, (c, v) => c.WriteStringNullableUnmanagedDictionary(v), IntPtr.Size);

        public static void WriteStringStringDictionaryField(this BufferedObjectWriteContextScope scope, IDictionary<string, string> dictionary)
        => scope.WriteField(dictionary, (c, v) => c.WriteStringStringDictionary(v), IntPtr.Size);

        public static void WriteStringBinaryDictionaryField(this BufferedObjectWriteContextScope scope, IDictionary<string, byte[]> dictionary)
        => scope.WriteField(dictionary, (c, v) => c.WriteStringBinaryDictionary(v), IntPtr.Size);

        public static void WriteStringBinaryDictionaryField(this BufferedObjectWriteContextScope scope, IDictionary<string, Memory<byte>> dictionary)
        => scope.WriteField(dictionary, (c, v) => c.WriteStringBinaryDictionary(v), IntPtr.Size);

        public static void WriteStringBufferedObjectDictionaryField<TValue>(this BufferedObjectWriteContextScope scope, IDictionary<string, TValue> dictionary)
          where TValue : IBufferedObjectSource
       => scope.WriteField(dictionary, (c, v) => c.WriteStringBufferedObjectDictionary(v), IntPtr.Size);
        #endregion

        #region Dictionary
        public static void WriteUnmanagedNonNullableUnmanagedDictionaryField<TKey, TValue>(this BufferedObjectWriteContextScope scope, Dictionary<TKey, TValue> dictionary)
           where TKey : unmanaged, IComparable<TKey>
           where TValue : unmanaged
        => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedNonNullableUnmanagedDictionary(v), IntPtr.Size);

        public static void WriteUnmanagedNullableUnmanagedDictionaryField<TKey, TValue>(this BufferedObjectWriteContextScope scope, Dictionary<TKey, TValue?> dictionary)
           where TKey : unmanaged, IComparable<TKey>
           where TValue : unmanaged
        => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedNullableUnmanagedDictionary(v), IntPtr.Size);

        public static void WriteUnmanagedStringDictionaryField<TKey>(this BufferedObjectWriteContextScope scope, Dictionary<TKey, string> dictionary)
           where TKey : unmanaged, IComparable<TKey>
        => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedStringDictionary(v), IntPtr.Size);

        public static void WriteUnmanagedBinaryDictionaryField<TKey>(this BufferedObjectWriteContextScope scope, Dictionary<TKey, byte[]> dictionary)
          where TKey : unmanaged, IComparable<TKey>
        => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedBinaryDictionary(v), IntPtr.Size);

        public static void WriteUnmanagedBinaryDictionaryField<TKey>(this BufferedObjectWriteContextScope scope, Dictionary<TKey, Memory<byte>> dictionary)
         where TKey : unmanaged, IComparable<TKey>
        => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedBinaryDictionary(v), IntPtr.Size);

        public static void WriteUnmanagedBufferedObjectDictionaryField<TKey, TValue>(this BufferedObjectWriteContextScope scope, Dictionary<TKey, TValue> dictionary)
          where TKey : unmanaged, IComparable<TKey>
          where TValue : IBufferedObjectSource
       => scope.WriteField(dictionary, (c, v) => c.WriteUnmanagedBufferedObjectDictionary(v), IntPtr.Size);

        public static void WriteStringNonNullableUnmanagedDictionaryField<TValue>(this BufferedObjectWriteContextScope scope, Dictionary<string, TValue> dictionary)
          where TValue : unmanaged
       => scope.WriteField(dictionary, (c, v) => c.WriteStringNonNullableUnmanagedDictionary(v), IntPtr.Size);

        public static void WriteStringNullableUnmanagedDictionaryField<TValue>(this BufferedObjectWriteContextScope scope, Dictionary<string, TValue?> dictionary)
         where TValue : unmanaged
      => scope.WriteField(dictionary, (c, v) => c.WriteStringNullableUnmanagedDictionary(v), IntPtr.Size);

        public static void WriteStringStringDictionaryField(this BufferedObjectWriteContextScope scope, Dictionary<string, string> dictionary)
        => scope.WriteField(dictionary, (c, v) => c.WriteStringStringDictionary(v), IntPtr.Size);

        public static void WriteStringBinaryDictionaryField(this BufferedObjectWriteContextScope scope, Dictionary<string, byte[]> dictionary)
        => scope.WriteField(dictionary, (c, v) => c.WriteStringBinaryDictionary(v), IntPtr.Size);

        public static void WriteStringBinaryDictionaryField(this BufferedObjectWriteContextScope scope, Dictionary<string, Memory<byte>> dictionary)
        => scope.WriteField(dictionary, (c, v) => c.WriteStringBinaryDictionary(v), IntPtr.Size);

        public static void WriteStringBufferedObjectDictionaryField<TValue>(this BufferedObjectWriteContextScope scope, Dictionary<string, TValue> dictionary)
          where TValue : IBufferedObjectSource
       => scope.WriteField(dictionary, (c, v) => c.WriteStringBufferedObjectDictionary(v), IntPtr.Size);
        #endregion
    }
}
