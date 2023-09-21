using System.Runtime.CompilerServices;

namespace NativeBuffering.NewDictionaries
{
    internal unsafe readonly struct UnmanagedUnmanagedPair<TKey, TValue> : IReadOnlyBufferedObject<UnmanagedUnmanagedPair<TKey, TValue>>
       where TKey : unmanaged
       where TValue : unmanaged
    {
        private static readonly int _valueOffset = sizeof(TKey) % AlignmentCalculator.AlignmentOf<TValue>() == 0 ? sizeof(TKey) : sizeof(TKey) + AlignmentCalculator.AlignmentOf<TValue>() - sizeof(TKey) % AlignmentCalculator.AlignmentOf<TValue>();

        public UnmanagedUnmanagedPair(NativeBuffer buffer) => Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public static UnmanagedUnmanagedPair<TKey, TValue> Parse(NativeBuffer buffer) => new(buffer);
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<TKey, TValue> AsKeyValuePair() => new(Key, Value);
    }

    internal unsafe readonly struct UnmanagedBufferedObjectPair<TKey, TValue> : IReadOnlyBufferedObject<UnmanagedBufferedObjectPair<TKey, TValue>>
        where TKey : unmanaged
        where TValue : IReadOnlyBufferedObject<TValue>
    {
        private static readonly int _valueOffset = sizeof(TKey) % IntPtr.Size == 0 ? sizeof(TKey) : sizeof(TKey) + IntPtr.Size - sizeof(TKey) % IntPtr.Size;

        public UnmanagedBufferedObjectPair(NativeBuffer buffer) => Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public static UnmanagedBufferedObjectPair<TKey, TValue> Parse(NativeBuffer buffer) => new(buffer);
        public ref TKey Key
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.AsRef<TKey>(Buffer.Start);
        }
        public TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TValue.Parse(Buffer.CreateByOffset(_valueOffset));
        }
        public KeyValuePair<TKey, TValue> AsKeyValuePair() => new(Key, Value);
    }

    internal unsafe readonly struct StringUnmanagedPair<TValue> : IReadOnlyBufferedObject<StringUnmanagedPair<TValue>>
        where TValue : unmanaged
    {
        public StringUnmanagedPair(NativeBuffer buffer) => Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public string Key   {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BufferedString.Parse(Buffer);
        }
        public ref TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var valueAlignment = AlignmentCalculator.AlignmentOf<TValue>();
                var keySize = BufferedString.CalculateSize(Buffer.Start);
                var offset = keySize % valueAlignment == 0 ? keySize : keySize + valueAlignment - keySize % valueAlignment;
                return ref Unsafe.AsRef<TValue>(Buffer.GetPointerByOffset(offset));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringUnmanagedPair<TValue> Parse(NativeBuffer buffer) => new(buffer);
    }

    internal unsafe readonly struct StringBufferedObjectPair<TValue> : IReadOnlyBufferedObject<StringBufferedObjectPair<TValue>>
        where TValue : IReadOnlyBufferedObject<TValue>
    {
        public StringBufferedObjectPair(NativeBuffer buffer) => Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public string Key {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BufferedString.Parse(Buffer);
        }
        public TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var keySize = BufferedString.CalculateSize(Buffer.Start);
                var offset = keySize % IntPtr.Size == 0 ? keySize : keySize + IntPtr.Size - keySize % IntPtr.Size;
                return TValue.Parse(Buffer.CreateByOffset(offset));
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBufferedObjectPair<TValue> Parse(NativeBuffer buffer) => new(buffer);
    }
}
