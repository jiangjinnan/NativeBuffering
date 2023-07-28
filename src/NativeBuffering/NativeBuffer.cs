using System.Runtime.CompilerServices;

namespace NativeBuffering
{
public unsafe readonly struct NativeBuffer
{
    public byte[] Bytes { get; }
    public void* Start { get; }

    public NativeBuffer(byte[] bytes, void* start)
    {
        Bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
        Start = start;
    }

    public NativeBuffer(byte[] bytes, int index = 0)
    {
        Bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
        Start = Unsafe.AsPointer(ref bytes[index]);
    }

    public void* GetPointerByIndex(int index) => Unsafe.AsPointer(ref Bytes[index]);
    public void* GetPointerByOffset(int offset) => Unsafe.Add<byte>(Start, offset);
    public NativeBuffer CreateByIndex(int index) => new(Bytes, index);
    public NativeBuffer CreateByPointer(void* start) => new(Bytes, start);
    public NativeBuffer CreateByOffset(int offset) => new(Bytes, Unsafe.Add<byte>(Start, offset));
}
}
