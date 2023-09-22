namespace NativeBuffering
{
    public static class BufferedMessageWriteContextScopeCollectionExtensions
    {
        public static void WriteUnmanagedCollectionField<T>(this BufferedObjectWriteContextScope scope, IEnumerable<T> value) where T : unmanaged
        => scope.WriteField(value, (c, v) => c.WriteNonNullableUnmanagedCollection(v), IntPtr.Size);

        public static void WriteUnmanagedCollectionField<T>(this BufferedObjectWriteContextScope scope, IEnumerable<T?> value) where T : unmanaged
        => scope.WriteField(value, (c, v) => c.WriteNullableUnmanagedCollection(v), IntPtr.Size);

        public static void WriteStringCollectionField(this BufferedObjectWriteContextScope scope, IEnumerable<string> value)
        => scope.WriteField(value, (c, v) => c.WriteStringCollection(v), IntPtr.Size);

        public static void WriteBinaryCollectionField(this BufferedObjectWriteContextScope scope, IEnumerable<byte[]> value)
        => scope.WriteField(value, (c, v) => c.WriteBinaryCollection(v), IntPtr.Size);

        public static void WriteBinaryCollectionField(this BufferedObjectWriteContextScope scope, IEnumerable<Memory<byte>> value)
        => scope.WriteField(value, (c, v) => c.WriteBinaryCollection(v), IntPtr.Size);

        public static void WriteBufferedObjectCollectionField<T>(this BufferedObjectWriteContextScope scope, IEnumerable<T> value) where T : IBufferedObjectSource
        => scope.WriteField(value, (c, v) => c.WriteBufferedObjectCollection(v), IntPtr.Size);
    }
}
