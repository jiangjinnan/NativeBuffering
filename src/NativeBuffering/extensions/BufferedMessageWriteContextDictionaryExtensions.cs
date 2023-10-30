using NativeBuffering.Dictionaries;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace NativeBuffering
{
    public static unsafe class BufferedMessageWriteContextDictionaryExtensions
    {
        #region WriteUnmanagedNonNullableUnmanagedDictionary
        public static void WriteUnmanagedNonNullableUnmanagedDictionary<TKey, TValue>(this BufferedObjectWriteContext context, IDictionary<TKey, TValue> dictionary)
            where TKey : unmanaged, IComparable<TKey>
            where TValue : unmanaged
        {
            if(dictionary is Dictionary<TKey, TValue> dict)
            {
                WriteUnmanagedNonNullableUnmanagedDictionary(context, dict);
                return;
            }
            var kvEntries = Distribute(dictionary, out var entryCount);
            WriteUnmanagedNonNullableUnmanagedDictionary(context, kvEntries, dictionary.Count, entryCount);
        }

        public static void WriteUnmanagedNonNullableUnmanagedDictionary<TKey, TValue>(this BufferedObjectWriteContext context, Dictionary<TKey, TValue> dictionary)
            where TKey : unmanaged, IComparable<TKey>
            where TValue : unmanaged
        {
            var kvEntries = Distribute(dictionary, out var entryCount);
            WriteUnmanagedNonNullableUnmanagedDictionary(context, kvEntries, dictionary.Count, entryCount);
        }
        #endregion

        #region WriteUnmanagedNullableUnmanagedDictionary
        public static void WriteUnmanagedNullableUnmanagedDictionary<TKey, TValue>(this BufferedObjectWriteContext context, IDictionary<TKey, TValue?> dictionary)
           where TKey : unmanaged, IComparable<TKey>
           where TValue : unmanaged
           => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => ctx.WriteUnmanaged(value!.Value), AlignmentCalculator.AlignmentOf<TKey>(), AlignmentCalculator.AlignmentOf<TValue>(), value => value is null);
        public static void WriteUnmanagedNullableUnmanagedDictionary<TKey, TValue>(this BufferedObjectWriteContext context, Dictionary<TKey, TValue?> dictionary)
          where TKey : unmanaged, IComparable<TKey>
          where TValue : unmanaged
          => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => ctx.WriteUnmanaged(value!.Value), AlignmentCalculator.AlignmentOf<TKey>(), AlignmentCalculator.AlignmentOf<TValue>(), value => value is null);
        #endregion

        #region WriteUnmanagedStringDictionary
        public static void WriteUnmanagedStringDictionary<TKey>(this BufferedObjectWriteContext context, IDictionary<TKey, string> dictionary) where TKey : unmanaged, IComparable<TKey>
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => ctx.WriteString(value), AlignmentCalculator.AlignmentOf<TKey>(), IntPtr.Size, string.IsNullOrEmpty);
        public static void WriteUnmanagedStringDictionary<TKey>(this BufferedObjectWriteContext context, Dictionary<TKey, string> dictionary) where TKey : unmanaged, IComparable<TKey>
          => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => ctx.WriteString(value), AlignmentCalculator.AlignmentOf<TKey>(), IntPtr.Size, string.IsNullOrEmpty);
        #endregion

        #region WriteUnmanagedBinaryDictionary
        public static void WriteUnmanagedBinaryDictionary<TKey>(this BufferedObjectWriteContext context, IDictionary<TKey, byte[]> dictionary) where TKey : unmanaged, IComparable<TKey>
           => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => ctx.WriteBytes(value), AlignmentCalculator.AlignmentOf<TKey>(), IntPtr.Size, value => value is null || value.Length == 0);
        public static void WriteUnmanagedBinaryDictionary<TKey>(this BufferedObjectWriteContext context, Dictionary<TKey, byte[]> dictionary) where TKey : unmanaged, IComparable<TKey>
           => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => ctx.WriteBytes(value), AlignmentCalculator.AlignmentOf<TKey>(), IntPtr.Size, value => value is null || value.Length == 0);
        #endregion

        #region WriteUnmanagedBinaryDictionary
        public static void WriteUnmanagedBinaryDictionary<TKey>(this BufferedObjectWriteContext context, IDictionary<TKey, Memory<byte>> dictionary) where TKey : unmanaged, IComparable<TKey>
          => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => ctx.WriteBytes(value.Span), AlignmentCalculator.AlignmentOf<TKey>(), IntPtr.Size, value => value.Length == 0);
        public static void WriteUnmanagedBinaryDictionary<TKey>(this BufferedObjectWriteContext context, Dictionary<TKey, Memory<byte>> dictionary) where TKey : unmanaged, IComparable<TKey>
         => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => ctx.WriteBytes(value.Span), AlignmentCalculator.AlignmentOf<TKey>(), IntPtr.Size, value => value.Length == 0);
        #endregion

        #region WriteUnmanagedBufferedObjectDictionary
        public static void WriteUnmanagedBufferedObjectDictionary<TKey, TValue>(this BufferedObjectWriteContext context, IDictionary<TKey, TValue> dictionary)
            where TKey : unmanaged, IComparable<TKey>
            where TValue : IBufferedObjectSource
        => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => value.Write(ctx), AlignmentCalculator.AlignmentOf<TKey>(), IntPtr.Size, value => value is null);
        public static void WriteUnmanagedBufferedObjectDictionary<TKey, TValue>(this BufferedObjectWriteContext context, Dictionary<TKey, TValue> dictionary)
          where TKey : unmanaged, IComparable<TKey>
          where TValue : IBufferedObjectSource
      => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteUnmanaged(key), (ctx, value) => value.Write(ctx), AlignmentCalculator.AlignmentOf<TKey>(), IntPtr.Size, value => value is null);
        #endregion

        #region WriteStringNonNullableUnmanagedDictionary
        public static void WriteStringNonNullableUnmanagedDictionary<TValue>(this BufferedObjectWriteContext context, IDictionary<string, TValue> dictionary)
            where TValue : unmanaged
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteUnmanaged(value), IntPtr.Size, AlignmentCalculator.AlignmentOf<TValue>(), _ => false);
        public static void WriteStringNonNullableUnmanagedDictionary<TValue>(this BufferedObjectWriteContext context, Dictionary<string, TValue> dictionary)
          where TValue : unmanaged
          => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteUnmanaged(value), IntPtr.Size, AlignmentCalculator.AlignmentOf<TValue>(), _ => false);
        #endregion

        #region WriteStringNullableUnmanagedDictionary
        public static void WriteStringNullableUnmanagedDictionary<TValue>(this BufferedObjectWriteContext context, IDictionary<string, TValue?> dictionary)
            where TValue : unmanaged
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteUnmanaged(value!.Value), IntPtr.Size, AlignmentCalculator.AlignmentOf<TValue>(), value => value is null);
        public static void WriteStringNullableUnmanagedDictionary<TValue>(this BufferedObjectWriteContext context, Dictionary<string, TValue?> dictionary)
                   where TValue : unmanaged
                   => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteUnmanaged(value!.Value), IntPtr.Size, AlignmentCalculator.AlignmentOf<TValue>(), value => value is null);
        #endregion

        #region WriteStringStringDictionary
        public static void WriteStringStringDictionary(this BufferedObjectWriteContext context, IDictionary<string, string> dictionary)
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteString(value), IntPtr.Size, IntPtr.Size, value => string.IsNullOrEmpty(value));
        public static void WriteStringStringDictionary(this BufferedObjectWriteContext context, Dictionary<string, string> dictionary)
          => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteString(value), IntPtr.Size, IntPtr.Size, value => string.IsNullOrEmpty(value));
        #endregion

        #region WriteStringBinaryDictionary
        public static void WriteStringBinaryDictionary(this BufferedObjectWriteContext context, IDictionary<string, byte[]> dictionary)
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteBytes(value), IntPtr.Size, IntPtr.Size, value => value is null || value.Length == 0);
        public static void WriteStringBinaryDictionary(this BufferedObjectWriteContext context, Dictionary<string, byte[]> dictionary)
          => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteBytes(value), IntPtr.Size, IntPtr.Size, value => value is null || value.Length == 0);

        public static void WriteStringBinaryDictionary(this BufferedObjectWriteContext context, IDictionary<string, Memory<byte>> dictionary)
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteBytes(value.Span), IntPtr.Size, IntPtr.Size, value => value.Length == 0);

        public static void WriteStringBinaryDictionary(this BufferedObjectWriteContext context, Dictionary<string, Memory<byte>> dictionary)
         => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => ctx.WriteBytes(value.Span), IntPtr.Size, IntPtr.Size, value => value.Length == 0);
        #endregion

        #region WriteStringBufferedObjectDictionary
        public static void WriteStringBufferedObjectDictionary<TValue>(this BufferedObjectWriteContext context, IDictionary<string, TValue> dictionary)
            where TValue : IBufferedObjectSource
            => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => value.Write(ctx), IntPtr.Size, IntPtr.Size, value => value is null);

        public static void WriteStringBufferedObjectDictionary<TValue>(this BufferedObjectWriteContext context, Dictionary<string, TValue> dictionary)
          where TValue : IBufferedObjectSource
          => WriteVariableLengthDictionary(context, dictionary, (ctx, key) => ctx.WriteString(key), (ctx, value) => value.Write(ctx), IntPtr.Size, IntPtr.Size, value => value is null);
        #endregion

        #region Private methods
        private static void WriteVariableLengthDictionary<TKey, TValue>(
            this BufferedObjectWriteContext context,
            IDictionary<TKey, TValue> dictionary,
            Action<BufferedObjectWriteContext, TKey> keyWriter,
            Action<BufferedObjectWriteContext, TValue> valueWriter,
            int keyAligiment,
            int valueAlignment,
            Func<TValue, bool> defaultValueEvaluator) where TKey : notnull
        {
            if (dictionary is Dictionary<TKey, TValue> dict)
            {
                WriteVariableLengthDictionary(context, dict, keyWriter, valueWriter, keyAligiment, valueAlignment, defaultValueEvaluator);
                return;
            }

            var kvEntries = Distribute(dictionary, out var entryCount);
            WriteVariableLengthDictionary(context, kvEntries, dictionary.Count, entryCount, keyWriter, valueWriter, keyAligiment, valueAlignment, defaultValueEvaluator);
        }
        private static void WriteVariableLengthDictionary<TKey, TValue>(
           this BufferedObjectWriteContext context,
           Dictionary<TKey, TValue> dictionary,
           Action<BufferedObjectWriteContext, TKey> keyWriter,
           Action<BufferedObjectWriteContext, TValue> valueWriter,
           int keyAligiment,
           int valueAlignment,
           Func<TValue, bool> defaultValueEvaluator) where TKey : notnull
        {
            var kvEntries = Distribute(dictionary, out var entryCount);
            WriteVariableLengthDictionary(context, kvEntries, dictionary.Count, entryCount, keyWriter, valueWriter, keyAligiment, valueAlignment, defaultValueEvaluator);
        }
        private static PooledList<KeyValuePair<TKey, TValue>>?[] Distribute<TKey, TValue>(IDictionary<TKey, TValue> dictionary, out int entryCount)
        {
            entryCount = DictionaryUtilities.GetEntrySlotCount(dictionary);
            var group = ArrayPool<PooledList<KeyValuePair<TKey, TValue>>?>.Shared.Rent(entryCount);

            var count = dictionary.Count;
            foreach (var kv in dictionary)
            {
                var entryIndex = kv.Key.GetDictionaryEntryIndex(entryCount);
                var list = group[entryIndex] ??= PooledList<KeyValuePair<TKey, TValue>>.Rent(count);
                list.Add(kv);
            }
            return group;
        }

        private static PooledList<KeyValuePair<TKey, TValue>>?[] Distribute<TKey, TValue>(Dictionary<TKey, TValue> dictionary, out int entryCount) where TKey : notnull
        {
            entryCount = DictionaryUtilities.GetEntrySlotCount(dictionary);
            var group = ArrayPool<PooledList<KeyValuePair<TKey, TValue>>?>.Shared.Rent(entryCount);

            var count = dictionary.Count;
            foreach (var kv in dictionary)
            {
                var entryIndex = kv.Key.GetDictionaryEntryIndex(entryCount);
                var list = group[entryIndex] ??= PooledList<KeyValuePair<TKey, TValue>>.Rent(count);
                list.Add(kv);
            }
            return group;
        }

        private static void WriteUnmanagedNonNullableUnmanagedDictionary<TKey, TValue>(BufferedObjectWriteContext context, PooledList<KeyValuePair<TKey, TValue>>?[] kvEntries, int kvCount, int entryCount)
            where TKey : unmanaged, IComparable<TKey>
            where TValue : unmanaged
        {
            context.EnsureAlignment(IntPtr.Size);
            context.WriteUnmanaged(kvCount);
            context.WriteUnmanaged(entryCount);

            var indexStart = context.Position;
            context.Advance(entryCount * sizeof(int));
            for (var index = 0; index < entryCount; index++)
            {
                var kvs = kvEntries[index];
                if (kvs is null)
                {
                    if (!context.IsSizeCalculateMode)
                    {
                        var pointer = Unsafe.AsPointer(ref context.Bytes[indexStart + sizeof(int) * index]);
                        Unsafe.Write(pointer, -1);
                    }
                }
                else
                {
                    context.AddPaddingBytes(IntPtr.Size); // align to pointer size
                    context.WriteReference(indexStart, index, false);
                    context.WriteUnmanaged(kvs.Count);
                    context.Advance(IntPtr.Size - sizeof(int)); // align to pointer size
                    foreach (var item in kvs.ToArraySegment())
                    {
                        context.WriteUnmanaged(new UnmanangedKV<TKey, TValue>(item.Key, item.Value));
                    }
                    kvs.Return();
                }
            }
            ArrayPool<PooledList<KeyValuePair<TKey, TValue>>?>.Shared.Return(kvEntries);
        }

        private static void WriteKVs<TKey, TValue>(this BufferedObjectWriteContext context, ArraySegment<KeyValuePair<TKey, TValue>> kvs,
            Action<BufferedObjectWriteContext, TKey> keyWriter,
            Action<BufferedObjectWriteContext, TValue> valueWriter,
            int keyAligiment,
            int valueAlignment,
            Func<TValue, bool> defaultValueEvaluator)
        {
            var count = kvs.Count;
            context.WriteUnmanaged(count);
            var indexStart = context.Position;
            context.Advance(count * sizeof(int));

            var index = 0;
            foreach (var item in kvs)
            {
                var isDefaultValue = defaultValueEvaluator(item.Value);
                context.AddPaddingBytes(IntPtr.Size);
                context.WriteReference(indexStart, index++, false);
                context.WriteUnmanaged((byte)(isDefaultValue ? 0 : 1));

                context.AddPaddingBytes(keyAligiment);
                keyWriter(context, item.Key);

                if (!isDefaultValue)
                {
                    context.AddPaddingBytes(valueAlignment);
                    valueWriter(context, item.Value);
                }
            }
        }

        private static void WriteVariableLengthDictionary<TKey, TValue>(
          this BufferedObjectWriteContext context,
            PooledList<KeyValuePair<TKey, TValue>>?[] kvEntries,
            int kvCount,
            int entryCount,
          Action<BufferedObjectWriteContext, TKey> keyWriter,
          Action<BufferedObjectWriteContext, TValue> valueWriter,
          int keyAligiment,
          int valueAlignment,
          Func<TValue, bool> defaultValueEvaluator)
        {
            context.EnsureAlignment(IntPtr.Size);
            context.WriteUnmanaged(kvCount);
            context.WriteUnmanaged(entryCount);
            var indexStart = context.Position;
            context.Advance(entryCount * sizeof(int));
            for (var index = 0; index < entryCount; index++)
            {
                var kvs = kvEntries[index];
                if (kvs is null)
                {
                    if (!context.IsSizeCalculateMode)
                    {
                        var pointer = Unsafe.AsPointer(ref context.Bytes[indexStart + sizeof(int) * index]);
                        Unsafe.Write(pointer, -1);
                    }
                }
                else
                {
                    context.AddPaddingBytes(IntPtr.Size); // align to pointer size
                    context.WriteReference(indexStart, index, false);
                    context.WriteKVs(kvs.ToArraySegment(), keyWriter, valueWriter, keyAligiment, valueAlignment, defaultValueEvaluator);
                    kvs.Return();
                }
            }
            ArrayPool<PooledList<KeyValuePair<TKey, TValue>>?>.Shared.Return(kvEntries);
        }
        #endregion
    }
}