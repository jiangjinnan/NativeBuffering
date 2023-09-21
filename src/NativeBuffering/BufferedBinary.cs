using System.Runtime.CompilerServices;

namespace NativeBuffering
{
    public unsafe readonly struct BufferedBinary : IReadOnlyBufferedObject<BufferedBinary>
    {
        public BufferedBinary(NativeBuffer buffer) => Buffer = buffer;
        public NativeBuffer Buffer { get; }
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Unsafe.Read<int>(Buffer.Start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> AsSpan() => new(Buffer.GetPointerByOffset(sizeof(int)), Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BufferedBinary Parse(NativeBuffer buffer) => new(buffer);
    }
}
