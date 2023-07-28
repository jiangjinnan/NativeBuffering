using System.Runtime.CompilerServices;

namespace NativeBuffering.Dictionaries
{
    internal unsafe readonly struct UnmanagedUnmanagedPair<TKey, TValue> : IReadOnlyBufferedObject<UnmanagedUnmanagedPair<TKey, TValue>>
       where TKey : unmanaged
       where TValue : unmanaged
    {
        public UnmanagedUnmanagedPair(NativeBuffer buffer) => Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public static UnmanagedUnmanagedPair<TKey, TValue> Parse(NativeBuffer buffer) => new(buffer);
        public ref TKey Key => ref Unsafe.AsRef<TKey>(Buffer.Start);
        public ref TValue Value => ref Unsafe.AsRef<TValue>(Buffer.GetPointerByOffset(sizeof(TKey)));
        public KeyValuePair<TKey, TValue> AsKeyValuePair() => new(Key, Value);
        public static int CalculateSize() => sizeof(TKey) + sizeof(TValue);
    }

    internal unsafe readonly struct UnmanagedBufferedObjectPair<TKey, TValue> : IReadOnlyBufferedObject<UnmanagedBufferedObjectPair<TKey, TValue>>
        where TKey : unmanaged
        where TValue : IReadOnlyBufferedObject<TValue>
    {
        public UnmanagedBufferedObjectPair(NativeBuffer buffer)=> Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public static UnmanagedBufferedObjectPair<TKey, TValue> Parse(NativeBuffer buffer) => new(buffer);
        public ref TKey Key => ref Unsafe.AsRef<TKey>(Buffer.Start);
        public TValue Value => TValue.Parse(Buffer.CreateByOffset(sizeof(TKey)));
        public KeyValuePair<TKey, TValue> AsKeyValuePair() => new(Key, Value);
    }

    internal unsafe readonly struct StringUnmanagedPair<TValue> : IReadOnlyBufferedObject<StringUnmanagedPair<TValue>>
        where TValue : unmanaged
    {
        public StringUnmanagedPair(NativeBuffer buffer) => Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public string Key => BufferedString.Parse(Buffer);
        public ref TValue Value => ref Unsafe.AsRef<TValue>(Buffer.GetPointerByOffset(BufferedString.CalculateSize(Buffer.Start)));
        public static StringUnmanagedPair<TValue> Parse(NativeBuffer buffer) => new(buffer);
        public KeyValuePair<string, TValue> AsKeyValuePair() => new(Key, Value);
    }

    internal unsafe readonly struct StringBufferedObjectPair<TValue> : IReadOnlyBufferedObject<StringBufferedObjectPair<TValue>>
        where TValue : IReadOnlyBufferedObject<TValue>
    {
        public StringBufferedObjectPair(NativeBuffer buffer) => Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public string Key => BufferedString.Parse(Buffer);
        public TValue Value => TValue.Parse(Buffer.CreateByOffset(BufferedString.CalculateSize(Buffer.Start)));
        public static StringBufferedObjectPair<TValue> Parse(NativeBuffer buffer) => new(buffer);
        public KeyValuePair<string, TValue> AsKeyValuePair() => new(Key, Value);
    }
}
