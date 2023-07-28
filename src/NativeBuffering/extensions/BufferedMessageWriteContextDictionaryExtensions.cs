namespace NativeBuffering
{
    public static class BufferedMessageWriteContextDictionaryExtensions
    {
        public static void WriteUnmanagedUnmanagedDictionary<TKey, TValue>(this BufferedObjectWriteContext context, IDictionary<TKey, TValue> dictionary)
            where TKey : unmanaged, IComparable<TKey>
            where TValue : unmanaged
        {
            var count = dictionary.Count;
            context.WriteUnmanaged(count);
            foreach (var item in dictionary.OrderBy(it => it.Key))
            {
                context.WriteUnmanaged(item.Key);
                context.WriteUnmanaged(item.Value);
            }
        }

        public static void WriteUnmanagedStringDictionary<TKey>(this BufferedObjectWriteContext context, IDictionary<TKey, string> dictionary) where TKey : unmanaged, IComparable<TKey>
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => ctx.WriteString(value));

        public static void WriteUnmanagedBinaryDictionary<TKey>(this BufferedObjectWriteContext context, IDictionary<TKey, byte[]> dictionary) where TKey : unmanaged, IComparable<TKey>
           => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => ctx.WriteBytes(value));

        public static void WriteUnmanagedBinaryDictionary<TKey>(this BufferedObjectWriteContext context, IDictionary<TKey, Memory<byte>> dictionary) where TKey : unmanaged, IComparable<TKey>
          => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => ctx.WriteBytes(value.Span));

        public static void WriteUnmanagedBufferedObjectDictionary<TKey, TValue>(this BufferedObjectWriteContext context, IDictionary<TKey, TValue> dictionary)
            where TKey : unmanaged, IComparable<TKey>
            where TValue : IBufferedObjectSource
        => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => value.Write(ctx));

        public static void WriteStringUnmanagedDictionary<TValue>(this BufferedObjectWriteContext context, IDictionary<string, TValue> dictionary)
            where TValue : unmanaged
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteUnmanaged(value));
        public static void WriteStringStringDictionary(this BufferedObjectWriteContext context, IDictionary<string, string> dictionary)
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteString(value));

        public static void WriteStringBinaryDictionary(this BufferedObjectWriteContext context, IDictionary<string, byte[]> dictionary)
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteBytes(value));
        public static void WriteStringBinaryDictionary(this BufferedObjectWriteContext context, IDictionary<string, Memory<byte>> dictionary)
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteBytes(value.Span));

        public static void WriteStringBufferedObjectDictionary<TValue>(this BufferedObjectWriteContext context, IDictionary<string, TValue> dictionary)
            where TValue: IBufferedObjectSource
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => value.Write(ctx));

        public static void WriteVariableLengthDictionary<TKey, TValue>(
            this BufferedObjectWriteContext context,
            IDictionary<TKey, TValue> dictionary,
            Action<BufferedObjectWriteContext, TKey> keyWriter,
            Action<BufferedObjectWriteContext, TValue> valueWriter)
        {
            var count = dictionary.Count;
            context.WriteUnmanaged(count);
            var indexStart = context.Position;
            context.Advance(count * sizeof(int));
            var index = 0;
            foreach (var item in dictionary.OrderBy(it => it.Key))
            {
                context.WriteReference(indexStart, index++);
                keyWriter(context, item.Key);
                valueWriter(context, item.Value);
            }
        }
    }
}
