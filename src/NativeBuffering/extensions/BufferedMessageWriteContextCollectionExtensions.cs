using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NativeBuffering
{
    public unsafe static class BufferedMessageWriteContextCollectionExtensions
    {
        public static void WriteUnmanagedCollection<T>(this BufferedObjectWriteContext context ,IEnumerable<T> collection) where T : unmanaged
        {
            context.WriteUnmanaged(collection.Count());
            foreach (var item in collection)
            {
                context.WriteUnmanaged(item);
            }
        }

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
            var count = collection.Count();
            context.WriteUnmanaged(count);
            var indexStart = context.Position;
            context.Advance(count * sizeof(int));
            var index = 0;
            foreach (var item in collection)
            {
                WriteReference(context, indexStart, index++);
                elementWriter(context, item);
            }
        }

        internal static void WriteReference(this BufferedObjectWriteContext context, int start, int index)
        {
            var pointer = Unsafe.AsPointer(ref context.Bytes[start + sizeof(int) * index]);
            Unsafe.Write(pointer, context.Position);
        }
    }
}
