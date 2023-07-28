using System.Runtime.CompilerServices;

namespace NativeBuffering
{
public unsafe readonly struct BufferedBinary : IReadOnlyBufferedObject<BufferedBinary>
{
    public BufferedBinary(NativeBuffer buffer) => Buffer = buffer;
    public NativeBuffer Buffer { get; }
    public int Length => Unsafe.Read<int>(Buffer.Start);
    public ReadOnlySpan<byte> AsSpan() => new(Buffer.GetPointerByOffset(sizeof(int)), Length);
    public static BufferedBinary Parse(NativeBuffer buffer) => new(buffer);
}
}
