using System.Runtime.CompilerServices;

namespace NativeBuffering.NewDictionaries
{
    internal unsafe readonly struct UnmanagedNonNullableUnmanagedPair<TKey, TValue> : IReadOnlyBufferedObject<UnmanagedNonNullableUnmanagedPair<TKey, TValue>>
       where TKey : unmanaged
       where TValue : unmanaged
    {
        private static readonly int _valueOffset = sizeof(TKey) % AlignmentCalculator.AlignmentOf<TValue>() == 0 ? sizeof(TKey) : sizeof(TKey) + AlignmentCalculator.AlignmentOf<TValue>() - sizeof(TKey) % AlignmentCalculator.AlignmentOf<TValue>();
        public UnmanagedNonNullableUnmanagedPair(NativeBuffer buffer) => Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public static UnmanagedNonNullableUnmanagedPair<TKey, TValue> Parse(NativeBuffer buffer) => new(buffer);
        public ref TKey Key
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.AsRef<TKey>(Buffer.Start);
        }
        public ref TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.AsRef<TValue>(Buffer.GetPointerByOffset(_valueOffset));
        }

        public static UnmanagedNonNullableUnmanagedPair<TKey, TValue> DefaultValue => throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<TKey, TValue> AsKeyValuePair() => new(Key, Value);
    }

    internal unsafe readonly struct UnmanagedNullableUnmanagedPair<TKey, TValue> : IReadOnlyBufferedObject<UnmanagedNullableUnmanagedPair<TKey, TValue>>
       where TKey : unmanaged
       where TValue : unmanaged
    {
        private static readonly int _keyOffset = AlignmentCalculator.AlignmentOf<TKey>();
        private static readonly int _valueOffset = 2 * sizeof(TKey) % AlignmentCalculator.AlignmentOf<TValue>() == 0 
            ? 2 * sizeof(TKey) 
            : 2 * sizeof(TKey) + AlignmentCalculator.AlignmentOf<TValue>() - 2 * sizeof(TKey) % AlignmentCalculator.AlignmentOf<TValue>();

        public UnmanagedNullableUnmanagedPair(NativeBuffer buffer) => Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public static UnmanagedNullableUnmanagedPair<TKey, TValue> Parse(NativeBuffer buffer) => new(buffer);
        public bool HasValue => Unsafe.Read<byte>(Buffer.Start) == 1;
        public TKey Key
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Unsafe.Read<TKey>(Buffer.GetPointerByOffset(_keyOffset));
        }
        public TValue? Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => HasValue? Unsafe.Read<TValue>(Buffer.GetPointerByOffset(_valueOffset)): null;
        }
        public static UnmanagedNullableUnmanagedPair<TKey, TValue> DefaultValue => throw new NotImplementedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<TKey, TValue?> AsKeyValuePair() => new(Key, Value);
    }

    internal unsafe readonly struct UnmanagedNonNullableBufferedObjectPair<TKey, TValue> : IReadOnlyBufferedObject<UnmanagedNonNullableBufferedObjectPair<TKey, TValue>>
        where TKey : unmanaged
        where TValue : struct, IReadOnlyBufferedObject<TValue>
    {
        private static readonly int _keyOffset = AlignmentCalculator.AlignmentOf<TKey>();
        private static readonly int _valueOffset = 2 * sizeof(TKey) % IntPtr.Size == 0 
            ? 2 * sizeof(TKey) 
            : 2 * sizeof(TKey) + IntPtr.Size - 2 * sizeof(TKey) % IntPtr.Size;

        public UnmanagedNonNullableBufferedObjectPair(NativeBuffer buffer) => Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public static UnmanagedNonNullableBufferedObjectPair<TKey, TValue> Parse(NativeBuffer buffer) => new(buffer);
        public bool HasValue => Unsafe.Read<byte>(Buffer.Start) == 1;
        public ref TKey Key
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.AsRef<TKey>(Buffer.GetPointerByOffset(_keyOffset));
        }
        public TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => HasValue? TValue.Parse(Buffer.CreateByOffset(_valueOffset)): TValue.DefaultValue;
        }

        public static UnmanagedNonNullableBufferedObjectPair<TKey, TValue> DefaultValue => throw new NotImplementedException();
        public KeyValuePair<TKey, TValue> AsKeyValuePair() => new(Key, Value);
    }

    internal unsafe readonly struct UnmanagedNullableBufferedObjectPair<TKey, TValue> : IReadOnlyBufferedObject<UnmanagedNullableBufferedObjectPair<TKey, TValue>>
    where TKey : unmanaged
    where TValue : struct, IReadOnlyBufferedObject<TValue>
    {
        private static readonly int _keyOffset = AlignmentCalculator.AlignmentOf<TKey>();
        private static readonly int _valueOffset = 2 * sizeof(TKey) % IntPtr.Size == 0
            ? 2 * sizeof(TKey)
            : 2 * sizeof(TKey) + IntPtr.Size - 2 * sizeof(TKey) % IntPtr.Size;

        public UnmanagedNullableBufferedObjectPair(NativeBuffer buffer) => Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public static UnmanagedNullableBufferedObjectPair<TKey, TValue> Parse(NativeBuffer buffer) => new(buffer);
        public bool HasValue => Unsafe.Read<byte>(Buffer.Start) == 1;
        public ref TKey Key
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.AsRef<TKey>(Buffer.GetPointerByOffset(_keyOffset));
        }
        public TValue? Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => HasValue? TValue.Parse(Buffer.CreateByOffset(_valueOffset)) : null;
        }
        public static UnmanagedNullableBufferedObjectPair<TKey, TValue> DefaultValue => throw new NotImplementedException();
        public KeyValuePair<TKey, TValue?> AsKeyValuePair() => new(Key, Value);
    }

    internal unsafe readonly struct StringNonNullableUnmanagedPair<TValue> : IReadOnlyBufferedObject<StringNonNullableUnmanagedPair<TValue>>
        where TValue : unmanaged
    {
        public StringNonNullableUnmanagedPair(NativeBuffer buffer) => Buffer = buffer;
        public static StringNonNullableUnmanagedPair<TValue> DefaultValue => throw new NotImplementedException();
        public NativeBuffer Buffer { get; }
        public string Key   {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BufferedString.Parse(Buffer.CreateByOffset(IntPtr.Size));
        }
        public ref TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var valueAlignment = AlignmentCalculator.AlignmentOf<TValue>();
                var keyPlusValudIndicatorSize = BufferedString.CalculateSize(Buffer.GetPointerByOffset(IntPtr.Size)) + IntPtr.Size;
                var offset = keyPlusValudIndicatorSize % valueAlignment == 0 ? keyPlusValudIndicatorSize : keyPlusValudIndicatorSize + valueAlignment - keyPlusValudIndicatorSize % valueAlignment;
                return ref Unsafe.AsRef<TValue>(Buffer.GetPointerByOffset(offset));
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringNonNullableUnmanagedPair<TValue> Parse(NativeBuffer buffer) => new(buffer);
    }

    internal unsafe readonly struct StringNullableUnmanagedPair<TValue> : IReadOnlyBufferedObject<StringNullableUnmanagedPair<TValue>>
        where TValue : unmanaged
    {
        public StringNullableUnmanagedPair(NativeBuffer buffer) => Buffer = buffer;
        public static StringNullableUnmanagedPair<TValue> DefaultValue => throw new NotImplementedException();
        public NativeBuffer Buffer { get; }
        public bool HasValue => Unsafe.Read<byte>(Buffer.Start) == 1;
        public string Key
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BufferedString.Parse(Buffer.CreateByOffset(IntPtr.Size));
        }
        public TValue? Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!HasValue) return null;
                var valueAlignment = AlignmentCalculator.AlignmentOf<TValue>();
                var keyPlusValudIndicatorSize = BufferedString.CalculateSize(Buffer.GetPointerByOffset(IntPtr.Size)) + IntPtr.Size;
                var offset = keyPlusValudIndicatorSize % valueAlignment == 0 ? keyPlusValudIndicatorSize : keyPlusValudIndicatorSize + valueAlignment - keyPlusValudIndicatorSize % valueAlignment;
                return  Unsafe.Read<TValue>(Buffer.GetPointerByOffset(offset));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringNullableUnmanagedPair<TValue> Parse(NativeBuffer buffer) => new(buffer);
    }

    internal unsafe readonly struct StringNonNullableBufferedObjectPair<TValue> : IReadOnlyBufferedObject<StringNonNullableBufferedObjectPair<TValue>>
        where TValue : IReadOnlyBufferedObject<TValue>
    {
        public StringNonNullableBufferedObjectPair(NativeBuffer buffer) => Buffer = buffer;
        public static StringNonNullableBufferedObjectPair<TValue> DefaultValue => throw new NotImplementedException();
        public NativeBuffer Buffer { get; }
        public bool HasValue => Unsafe.Read<byte>(Buffer.Start) == 1;
        public string Key {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BufferedString.Parse(Buffer.CreateByOffset(IntPtr.Size));
        }
        public TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!HasValue) return TValue.DefaultValue;
                var keyPlusValudIndicatorSize = BufferedString.CalculateSize(Buffer.GetPointerByOffset(IntPtr.Size)) + IntPtr.Size;
                var offset = keyPlusValudIndicatorSize % IntPtr.Size == 0 ? keyPlusValudIndicatorSize : keyPlusValudIndicatorSize + IntPtr.Size - keyPlusValudIndicatorSize % IntPtr.Size;
                return TValue.Parse(Buffer.CreateByOffset(offset));
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringNonNullableBufferedObjectPair<TValue> Parse(NativeBuffer buffer) => new(buffer);
    }

    internal unsafe readonly struct StringNullableBufferedObjectPair<TValue> : IReadOnlyBufferedObject<StringNullableBufferedObjectPair<TValue>>
        where TValue :struct, IReadOnlyBufferedObject<TValue>
    {
        public StringNullableBufferedObjectPair(NativeBuffer buffer) => Buffer = buffer;
        public static StringNullableBufferedObjectPair<TValue> DefaultValue => throw new NotImplementedException();
        public NativeBuffer Buffer { get; }
        public bool HasValue => Unsafe.Read<byte>(Buffer.Start) == 1;
        public string Key
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BufferedString.Parse(Buffer.CreateByOffset(IntPtr.Size));
        }
        public TValue? Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if(!HasValue) return null;
                var keyPlusValudIndicatorSize = BufferedString.CalculateSize(Buffer.GetPointerByOffset(IntPtr.Size)) + IntPtr.Size;
                var offset = keyPlusValudIndicatorSize % IntPtr.Size == 0 ? keyPlusValudIndicatorSize : keyPlusValudIndicatorSize + IntPtr.Size - keyPlusValudIndicatorSize % IntPtr.Size;
                return TValue.Parse(Buffer.CreateByOffset(offset));
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringNullableBufferedObjectPair<TValue> Parse(NativeBuffer buffer) => new(buffer);
    }
}
