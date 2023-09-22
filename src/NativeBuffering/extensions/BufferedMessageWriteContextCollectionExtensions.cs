using System.Runtime.CompilerServices;

namespace NativeBuffering
{
    public unsafe static class BufferedMessageWriteContextCollectionExtensions
    {
        public static void WriteNonNullableUnmanagedCollection<T>(this BufferedObjectWriteContext context, IEnumerable<T> collection) where T : unmanaged
        {
            context.EnsureAlignment(IntPtr.Size);
            context.WriteUnmanaged(collection.Count());
            context.Advance(IntPtr.Size - sizeof(int));
            foreach (var item in collection)
            {
                context.AddPaddingBytes(AlignmentCalculator.AlignmentOf<T>());
                context.WriteUnmanaged(item);
            }
        }

        public static void WriteNullableUnmanagedCollection<T>(this BufferedObjectWriteContext context, IEnumerable<T?> collection) where T : unmanaged
         => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteUnmanaged(element!.Value));

        public static void WriteStringCollection(this BufferedObjectWriteContext context, IEnumerable<string> collection)
            => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteString(element));

        public static void WriteBinaryCollection(this BufferedObjectWriteContext context, IEnumerable<byte[]> collection)
            => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteBytes(element));

        public static void WriteBinaryCollection(this BufferedObjectWriteContext context, IEnumerable<Memory<byte>> collection)
            => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteBytes(element.Span));

        public static void WriteBufferedObjectCollection<T>(this BufferedObjectWriteContext context, IEnumerable<T> collection) where T : IBufferedObjectSource
          => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => element.Write(ctx));

        public static void WriteVariableLengthTypedCollection<T>(this BufferedObjectWriteContext context, IEnumerable<T> collection, Action<BufferedObjectWriteContext, T> elementWriter)
        {
            collection??=Enumerable.Empty<T>();
            context.EnsureAlignment(IntPtr.Size);
            var count = collection.Count();
            context.WriteUnmanaged(count);
            var indexStart = context.Position;
            context.Advance(count * sizeof(int));
            var index = 0;
            foreach (var item in collection)
            {
                if (item is null ||(item is string stringValue && string.IsNullOrEmpty(stringValue)))
                {
                    WriteReference(context, indexStart, index++, true);
                    continue;
                }
                context.AddPaddingBytes(IntPtr.Size);
                WriteReference(context, indexStart, index++, false);
                elementWriter(context, item);
            }
        }

        internal static void WriteReference(this BufferedObjectWriteContext context, int start, int index, bool isNull)
        {
            if (!context.IsSizeCalculateMode)
            {
                var pointer = Unsafe.AsPointer(ref context.Bytes[start + sizeof(int) * index]);
                Unsafe.Write(pointer, isNull ? -1 : context.Position);
            }
        }
    }
}
