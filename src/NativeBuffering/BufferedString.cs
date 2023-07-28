using System.Runtime.CompilerServices;

namespace NativeBuffering
{
public unsafe readonly struct BufferedString : IReadOnlyBufferedObject<BufferedString>
{
    private readonly void* _start;
    public BufferedString(NativeBuffer buffer) => _start = buffer.Start;
    public BufferedString(void* start)=> _start = start;
    public static BufferedString Parse(NativeBuffer buffer) => new(buffer);
    public static BufferedString Parse(void* start) => new(start);
    public static int CalculateSize(void* start) => Unsafe.Read<int>(start);
    public string AsString()
    {
        string v = default!;
        Unsafe.Write(Unsafe.AsPointer(ref v), new IntPtr(Unsafe.Add<byte>(_start, sizeof(int) + IntPtr.Size)));
        return v;
    }
    public static implicit operator string(BufferedString value) => value.AsString();
    public override string ToString() => AsString();
}
}
