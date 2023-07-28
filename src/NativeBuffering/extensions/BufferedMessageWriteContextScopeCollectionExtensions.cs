namespace NativeBuffering
{
    public static class BufferedMessageWriteContextScopeCollectionExtensions
    {
        public static void WriteUnmanagedCollectionField<T>(this BufferedObjectWriteContextScope scope, IEnumerable<T> value) where T : unmanaged
        => scope.WriteField(value, (c, v) => c.WriteUnmanagedCollection(v));

        public static void WriteStringCollectionField(this BufferedObjectWriteContextScope scope, IEnumerable<string> value)
        => scope.WriteField(value, (c, v) => c.WriteStringCollection(v));

        public static void WriteBinaryCollectionField(this BufferedObjectWriteContextScope scope, IEnumerable<byte[]> value)
        => scope.WriteField(value, (c, v) => c.WriteBinaryCollection(v));

        public static void WriteBinaryCollectionField(this BufferedObjectWriteContextScope scope, IEnumerable<Memory<byte>> value)
        => scope.WriteField(value, (c, v) => c.WriteBinaryCollection(v));

        public static void WriteBufferedObjectCollectionField<T>(this BufferedObjectWriteContextScope scope, IEnumerable<T> value) where T : IBufferedObjectSource
        => scope.WriteField(value, (c, v) => c.WriteBufferedObjectCollection(v));
    }
}
