using System.Runtime.CompilerServices;

namespace NativeBuffering
{
    public unsafe static class BufferedMessageWriteContextCollectionExtensions
    {
        #region WriteNonNullableUnmanagedCollection
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

        public static void WriteNonNullableUnmanagedCollection<T>(this BufferedObjectWriteContext context, T[] collection) where T : unmanaged
        {
            context.EnsureAlignment(IntPtr.Size);
            context.WriteUnmanaged(collection.Length);
            context.Advance(IntPtr.Size - sizeof(int));
            for (int index = 0; index < collection.Length; index++)
            {
                context.AddPaddingBytes(AlignmentCalculator.AlignmentOf<T>());
                context.WriteUnmanaged(collection[index]);
            }
        }

        public static void WriteNonNullableUnmanagedCollection<T>(this BufferedObjectWriteContext context, IList<T> collection) where T : unmanaged
        {
            context.EnsureAlignment(IntPtr.Size);
            context.WriteUnmanaged(collection.Count);
            context.Advance(IntPtr.Size - sizeof(int));
            for (int index = 0; index < collection.Count; index++)
            {
                context.AddPaddingBytes(AlignmentCalculator.AlignmentOf<T>());
                context.WriteUnmanaged(collection[index]);
            }
        }

        public static void WriteNonNullableUnmanagedCollection<T>(this BufferedObjectWriteContext context, ICollection<T> collection) where T : unmanaged
        {
            context.EnsureAlignment(IntPtr.Size);
            context.WriteUnmanaged(collection.Count);
            context.Advance(IntPtr.Size - sizeof(int));
            foreach (var item in collection)
            {
                context.AddPaddingBytes(AlignmentCalculator.AlignmentOf<T>());
                context.WriteUnmanaged(item);
            }
        }
        #endregion

        #region WriteNullableUnmanagedCollection
        public static void WriteNullableUnmanagedCollection<T>(this BufferedObjectWriteContext context, IEnumerable<T?> collection) where T : unmanaged
         => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteUnmanaged(element!.Value));

        public static void WriteNullableUnmanagedCollection<T>(this BufferedObjectWriteContext context, T?[] collection) where T : unmanaged
        => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteUnmanaged(element!.Value));

        public static void WriteNullableUnmanagedCollection<T>(this BufferedObjectWriteContext context, IList<T?> collection) where T : unmanaged
       => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteUnmanaged(element!.Value));

        public static void WriteNullableUnmanagedCollection<T>(this BufferedObjectWriteContext context, ICollection<T?> collection) where T : unmanaged
       => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteUnmanaged(element!.Value));
        public static void WriteNullableUnmanagedCollection<T>(this BufferedObjectWriteContext context, Span<T?> collection) where T : unmanaged
       => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteUnmanaged(element!.Value));
        #endregion

        #region WriteStringCollection
        public static void WriteStringCollection(this BufferedObjectWriteContext context, IEnumerable<string> collection)
            => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteString(element));
        public static void WriteStringCollection(this BufferedObjectWriteContext context, IList<string> collection)
            => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteString(element));
        public static void WriteStringCollection(this BufferedObjectWriteContext context, string[] collection)
            => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteString(element));
        public static void WriteStringCollection(this BufferedObjectWriteContext context, ICollection<string> collection)
            => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteString(element));
        #endregion

        #region WriteBinaryCollection
        public static void WriteBinaryCollection(this BufferedObjectWriteContext context, IEnumerable<byte[]> collection)
            => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteBytes(element));
        public static void WriteBinaryCollection(this BufferedObjectWriteContext context, IList<byte[]> collection)
           => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteBytes(element));
        public static void WriteBinaryCollection(this BufferedObjectWriteContext context, ICollection<byte[]> collection)
           => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteBytes(element));
        public static void WriteBinaryCollection(this BufferedObjectWriteContext context, byte[][] collection)
           => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteBytes(element));

        public static void WriteBinaryCollection(this BufferedObjectWriteContext context, IEnumerable<Memory<byte>> collection)
            => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteBytes(element.Span));
        public static void WriteBinaryCollection(this BufferedObjectWriteContext context, IList<Memory<byte>> collection)
            => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteBytes(element.Span));
        public static void WriteBinaryCollection(this BufferedObjectWriteContext context, ICollection<Memory<byte>> collection)
            => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteBytes(element.Span));
        public static void WriteBinaryCollection(this BufferedObjectWriteContext context, Memory<byte>[] collection)
            => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => ctx.WriteBytes(element.Span));

        #endregion

        #region WriteBufferedObjectCollection
        public static void WriteBufferedObjectCollection<T>(this BufferedObjectWriteContext context, IEnumerable<T> collection) where T : IBufferedObjectSource
          => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => element.Write(ctx));
        public static void WriteBufferedObjectCollection<T>(this BufferedObjectWriteContext context, IList<T> collection) where T : IBufferedObjectSource
         => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => element.Write(ctx));
        public static void WriteBufferedObjectCollection<T>(this BufferedObjectWriteContext context, ICollection<T> collection) where T : IBufferedObjectSource
         => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => element.Write(ctx));
        public static void WriteBufferedObjectCollection<T>(this BufferedObjectWriteContext context, Span<T> collection) where T : IBufferedObjectSource
         => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => element.Write(ctx));
        public static void WriteBufferedObjectCollection<T>(this BufferedObjectWriteContext context, T[] collection) where T : IBufferedObjectSource
         => context.WriteVariableLengthTypedCollection(collection, (ctx, element) => element.Write(ctx));
        #endregion

        #region WriteVariableLengthTypedCollection
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

        public static void WriteVariableLengthTypedCollection<T>(this BufferedObjectWriteContext context, T[] collection, Action<BufferedObjectWriteContext, T> elementWriter)
        {
            collection ??= Array.Empty<T>();
            context.EnsureAlignment(IntPtr.Size);
            var count = collection.Length;
            context.WriteUnmanaged(count);
            var indexStart = context.Position;
            context.Advance(count * sizeof(int));
            for (int index = 0; index < collection.Length; index++)
            {
                var item = collection[index];
                if (item is null || (item is string stringValue && string.IsNullOrEmpty(stringValue)))
                {
                    WriteReference(context, indexStart, index, true);
                    continue;
                }
                context.AddPaddingBytes(IntPtr.Size);
                WriteReference(context, indexStart, index, false);
                elementWriter(context, item);
            }
        }

        public static void WriteVariableLengthTypedCollection<T>(this BufferedObjectWriteContext context, IList<T>? collection, Action<BufferedObjectWriteContext, T> elementWriter)
        {
            context.EnsureAlignment(IntPtr.Size);
            var count = collection?.Count??0;
            context.WriteUnmanaged(count);
            var indexStart = context.Position;
            context.Advance(count * sizeof(int));
            for (int index = 0; index < count; index++)
            {
                var item = collection![index];
                if (item is null || (item is string stringValue && string.IsNullOrEmpty(stringValue)))
                {
                    WriteReference(context, indexStart, index, true);
                    continue;
                }
                context.AddPaddingBytes(IntPtr.Size);
                WriteReference(context, indexStart, index, false);
                elementWriter(context, item);
            }
        }

        public static void WriteVariableLengthTypedCollection<T>(this BufferedObjectWriteContext context, ICollection<T>? collection, Action<BufferedObjectWriteContext, T> elementWriter)
        {
            context.EnsureAlignment(IntPtr.Size);
            var count = collection?.Count ?? 0;
            context.WriteUnmanaged(count);
            var indexStart = context.Position;
            context.Advance(count * sizeof(int));
            if (collection is not null)
            {
                var index = 0;
                foreach (var item in collection)
                {
                    if (item is null || (item is string stringValue && string.IsNullOrEmpty(stringValue)))
                    {
                        WriteReference(context, indexStart, index++, true);
                        continue;
                    }
                    context.AddPaddingBytes(IntPtr.Size);
                    WriteReference(context, indexStart, index++, false);
                    elementWriter(context, item);
                }
            }
        }

        public static void WriteVariableLengthTypedCollection<T>(this BufferedObjectWriteContext context, Span<T> collection, Action<BufferedObjectWriteContext, T> elementWriter)
        {
            context.EnsureAlignment(IntPtr.Size);
            var count = collection.Length;
            context.WriteUnmanaged(count);
            var indexStart = context.Position;
            context.Advance(count * sizeof(int));
            for (int index = 0; index < count; index++)
            {
                var item = collection[index];
                if (item is null || (item is string stringValue && string.IsNullOrEmpty(stringValue)))
                {
                    WriteReference(context, indexStart, index, true);
                    continue;
                }
                context.AddPaddingBytes(IntPtr.Size);
                WriteReference(context, indexStart, index, false);
                elementWriter(context, item);
            }
        }
        #endregion

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
