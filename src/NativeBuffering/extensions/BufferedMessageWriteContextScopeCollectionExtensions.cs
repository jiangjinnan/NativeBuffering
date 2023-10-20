namespace NativeBuffering
{
    public static class BufferedMessageWriteContextScopeCollectionExtensions
    {
        #region WriteUnmanagedCollectionField
        public static void WriteUnmanagedCollectionField<T>(this BufferedObjectWriteContextScope scope, IEnumerable<T> value) where T : unmanaged
        => scope.WriteField(value, (c, v) => c.WriteNonNullableUnmanagedCollection(v), IntPtr.Size);
        public static void WriteUnmanagedCollectionField<T>(this BufferedObjectWriteContextScope scope, IList<T> value) where T : unmanaged
       => scope.WriteField(value, (c, v) => c.WriteNonNullableUnmanagedCollection(v), IntPtr.Size);
        public static void WriteUnmanagedCollectionField<T>(this BufferedObjectWriteContextScope scope, ICollection<T> value) where T : unmanaged
       => scope.WriteField(value, (c, v) => c.WriteNonNullableUnmanagedCollection(v), IntPtr.Size);
        public static void WriteUnmanagedCollectionField<T>(this BufferedObjectWriteContextScope scope, T[] value) where T : unmanaged
       => scope.WriteField(value, (c, v) => c.WriteNonNullableUnmanagedCollection(v), IntPtr.Size);
        #endregion

        #region WriteUnmanagedCollectionField
        public static void WriteUnmanagedCollectionField<T>(this BufferedObjectWriteContextScope scope, IEnumerable<T?> value) where T : unmanaged
        => scope.WriteField(value, (c, v) => c.WriteNullableUnmanagedCollection(v), IntPtr.Size);
        public static void WriteUnmanagedCollectionField<T>(this BufferedObjectWriteContextScope scope, IList<T?> value) where T : unmanaged
       => scope.WriteField(value, (c, v) => c.WriteNullableUnmanagedCollection(v), IntPtr.Size);
        public static void WriteUnmanagedCollectionField<T>(this BufferedObjectWriteContextScope scope, ICollection<T?> value) where T : unmanaged
       => scope.WriteField(value, (c, v) => c.WriteNullableUnmanagedCollection(v), IntPtr.Size);
        public static void WriteUnmanagedCollectionField<T>(this BufferedObjectWriteContextScope scope, T?[] value) where T : unmanaged
       => scope.WriteField(value, (c, v) => c.WriteNullableUnmanagedCollection(v), IntPtr.Size);
        #endregion

        #region WriteStringCollectionField
        public static void WriteStringCollectionField(this BufferedObjectWriteContextScope scope, IEnumerable<string> value)
        => scope.WriteField(value, (c, v) => c.WriteStringCollection(v), IntPtr.Size);
        public static void WriteStringCollectionField(this BufferedObjectWriteContextScope scope, IList<string> value)
       => scope.WriteField(value, (c, v) => c.WriteStringCollection(v), IntPtr.Size);
        public static void WriteStringCollectionField(this BufferedObjectWriteContextScope scope, ICollection<string> value)
       => scope.WriteField(value, (c, v) => c.WriteStringCollection(v), IntPtr.Size);
        public static void WriteStringCollectionField(this BufferedObjectWriteContextScope scope, string[] value)
       => scope.WriteField(value, (c, v) => c.WriteStringCollection(v), IntPtr.Size);
        #endregion

        #region WriteBinaryCollectionField
        public static void WriteBinaryCollectionField(this BufferedObjectWriteContextScope scope, IEnumerable<byte[]> value)
        => scope.WriteField(value, (c, v) => c.WriteBinaryCollection(v), IntPtr.Size);
        public static void WriteBinaryCollectionField(this BufferedObjectWriteContextScope scope, IList<byte[]> value)
      => scope.WriteField(value, (c, v) => c.WriteBinaryCollection(v), IntPtr.Size);
        public static void WriteBinaryCollectionField(this BufferedObjectWriteContextScope scope, ICollection<byte[]> value)
      => scope.WriteField(value, (c, v) => c.WriteBinaryCollection(v), IntPtr.Size);
        public static void WriteBinaryCollectionField(this BufferedObjectWriteContextScope scope, byte[][] value)
      => scope.WriteField(value, (c, v) => c.WriteBinaryCollection(v), IntPtr.Size);

        public static void WriteBinaryCollectionField(this BufferedObjectWriteContextScope scope, IEnumerable<Memory<byte>> value)
        => scope.WriteField(value, (c, v) => c.WriteBinaryCollection(v), IntPtr.Size);

        public static void WriteBinaryCollectionField(this BufferedObjectWriteContextScope scope, IList<Memory<byte>> value)
        => scope.WriteField(value, (c, v) => c.WriteBinaryCollection(v), IntPtr.Size);

        public static void WriteBinaryCollectionField(this BufferedObjectWriteContextScope scope, ICollection<Memory<byte>> value)
        => scope.WriteField(value, (c, v) => c.WriteBinaryCollection(v), IntPtr.Size);

        public static void WriteBinaryCollectionField(this BufferedObjectWriteContextScope scope, Memory<byte>[] value)
        => scope.WriteField(value, (c, v) => c.WriteBinaryCollection(v), IntPtr.Size);
        #endregion

        #region WriteBufferedObjectCollectionField
        public static void WriteBufferedObjectCollectionField<T>(this BufferedObjectWriteContextScope scope, IEnumerable<T> value) where T : IBufferedObjectSource
        => scope.WriteField(value, (c, v) => c.WriteBufferedObjectCollection(v), IntPtr.Size);
        public static void WriteBufferedObjectCollectionField<T>(this BufferedObjectWriteContextScope scope, IList<T> value) where T : IBufferedObjectSource
        => scope.WriteField(value, (c, v) => c.WriteBufferedObjectCollection(v), IntPtr.Size);
        public static void WriteBufferedObjectCollectionField<T>(this BufferedObjectWriteContextScope scope, ICollection<T> value) where T : IBufferedObjectSource
        => scope.WriteField(value, (c, v) => c.WriteBufferedObjectCollection(v), IntPtr.Size);
        public static void WriteBufferedObjectCollectionField<T>(this BufferedObjectWriteContextScope scope, T[] value) where T : IBufferedObjectSource
        => scope.WriteField(value, (c, v) => c.WriteBufferedObjectCollection(v), IntPtr.Size);
        #endregion
    }
}
