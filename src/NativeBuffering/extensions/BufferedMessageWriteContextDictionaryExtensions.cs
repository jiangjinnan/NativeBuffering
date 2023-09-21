using NativeBuffering.Dictionaries;
using System.Runtime.CompilerServices;

namespace NativeBuffering
{
    public static unsafe class BufferedMessageWriteContextDictionaryExtensions
    {
        public static void WriteUnmanagedUnmanagedDictionary<TKey, TValue>(this BufferedObjectWriteContext context, IDictionary<TKey, TValue> dictionary)
            where TKey : unmanaged, IComparable<TKey>
            where TValue : unmanaged
        {
            context.EnsureAlignment(IntPtr.Size);
            var count = dictionary.Count;
            context.WriteUnmanaged(count);

            var entrySlotCount = DictionaryUtilities.GetEntrySlotCount(dictionary);
            context.WriteUnmanaged(entrySlotCount);

            var indexStart = context.Position;
            context.Advance(entrySlotCount * sizeof(int));

            var group = dictionary.GroupBy(it => it.Key.GetDictionaryEntryIndex(entrySlotCount)).ToDictionary(it => it.Key, it => it);

            for (var index = 0; index < entrySlotCount; index++)
            {
                if (group.TryGetValue(index, out var kvs))
                {
                    context.AddPaddingBytes(IntPtr.Size); // align to pointer size
                    context.WriteReference(indexStart, index);
                    context.WriteUnmanaged(kvs.Count());
                    context.Advance(IntPtr.Size - sizeof(int)); // align to pointer size
                    foreach (var item in kvs)
                    {
                        context.WriteUnmanaged(new UnmanangedKV<TKey, TValue>(item.Key, item.Value));
                    }
                }
                else
                {
                    if (!context.IsSizeCalculateMode)
                    {
                        var pointer = Unsafe.AsPointer(ref context.Bytes[indexStart + sizeof(int) * index]);
                        Unsafe.Write(pointer, -1);
                    }
                }
            }
        }

        public static void WriteUnmanagedStringDictionary<TKey>(this BufferedObjectWriteContext context, IDictionary<TKey, string> dictionary) where TKey : unmanaged, IComparable<TKey>
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => ctx.WriteString(value), IntPtr.Size);

        public static void WriteUnmanagedBinaryDictionary<TKey>(this BufferedObjectWriteContext context, IDictionary<TKey, byte[]> dictionary) where TKey : unmanaged, IComparable<TKey>
           => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => ctx.WriteBytes(value), IntPtr.Size);

        public static void WriteUnmanagedBinaryDictionary<TKey>(this BufferedObjectWriteContext context, IDictionary<TKey, Memory<byte>> dictionary) where TKey : unmanaged, IComparable<TKey>
          => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => ctx.WriteBytes(value.Span), IntPtr.Size);

        public static void WriteUnmanagedBufferedObjectDictionary<TKey, TValue>(this BufferedObjectWriteContext context, IDictionary<TKey, TValue> dictionary)
            where TKey : unmanaged, IComparable<TKey>
            where TValue : IBufferedObjectSource
        => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => value.Write(ctx),  IntPtr.Size);

        public static void WriteStringUnmanagedDictionary<TValue>(this BufferedObjectWriteContext context, IDictionary<string, TValue> dictionary)
            where TValue : unmanaged
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteUnmanaged(value), AlignmentCalculator.AlignmentOf<TValue>());
        public static void WriteStringStringDictionary(this BufferedObjectWriteContext context, IDictionary<string, string> dictionary)
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteString(value), IntPtr.Size);

        public static void WriteStringBinaryDictionary(this BufferedObjectWriteContext context, IDictionary<string, byte[]> dictionary)
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteBytes(value),  IntPtr.Size);
        public static void WriteStringBinaryDictionary(this BufferedObjectWriteContext context, IDictionary<string, Memory<byte>> dictionary)
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteBytes(value.Span), IntPtr.Size);

        public static void WriteStringBufferedObjectDictionary<TValue>(this BufferedObjectWriteContext context, IDictionary<string, TValue> dictionary)
            where TValue : IBufferedObjectSource
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => value.Write(ctx),  IntPtr.Size);

        public static void WriteVariableLengthDictionary<TKey, TValue>(
            this BufferedObjectWriteContext context,
            IDictionary<TKey, TValue> dictionary,
            Action<BufferedObjectWriteContext, TKey> keyWriter,
            Action<BufferedObjectWriteContext, TValue> valueWriter,
            int valueAlignment)
        {
            context.EnsureAlignment(IntPtr.Size);

            var count = dictionary.Count;
            context.WriteUnmanaged(count);

            var entrySlotCount = DictionaryUtilities.GetEntrySlotCount(dictionary);
            context.WriteUnmanaged(entrySlotCount);

            var group = dictionary.GroupBy(it => it.Key.GetDictionaryEntryIndex(entrySlotCount)).ToDictionary(it => it.Key, it => it);

            var indexStart = context.Position;
            context.Advance(entrySlotCount * sizeof(int));

            for (var index = 0; index < entrySlotCount; index++)
            {
                if (group.TryGetValue(index, out var kvs))
                {
                    context.AddPaddingBytes(IntPtr.Size); // align to pointer size
                    context.WriteReference(indexStart, index);
                    context.WriteKVs(kvs, keyWriter, valueWriter, valueAlignment);
                }
                else
                {
                    if (!context.IsSizeCalculateMode)
                    {
                        var pointer = Unsafe.AsPointer(ref context.Bytes[indexStart + sizeof(int) * index]);
                        Unsafe.Write(pointer, -1);
                    }
                }
            }
        }

        private static void WriteKVs<TKey, TValue>(this BufferedObjectWriteContext context, IEnumerable<KeyValuePair<TKey, TValue>> kvs,
            Action<BufferedObjectWriteContext, TKey> keyWriter,
            Action<BufferedObjectWriteContext, TValue> valueWriter,
            int valueAlignment)
        {
            var count = kvs.Count();
            context.WriteUnmanaged(count);
            var indexStart = context.Position;
            context.Advance(count * sizeof(int));

            var index = 0;
            foreach (var item in kvs)
            {
                context.AddPaddingBytes(IntPtr.Size); 

                context.WriteReference(indexStart, index++);
                keyWriter(context, item.Key);
                context.AddPaddingBytes(valueAlignment);
                valueWriter(context, item.Value);
            }
        }        
    }
}
