using System.Runtime.CompilerServices;
using System.Text;

namespace NativeBuffering
{
    public unsafe readonly struct BufferedString : IReadOnlyBufferedObject<BufferedString>
    {
        private readonly void* _start;
        public static BufferedString DefaultValue { get; }
        static BufferedString()
        { 
            var size = CalculateStringSize(string.Empty);
            var bytes = new byte[size];

            var context = BufferedObjectWriteContext.Create(bytes);
            context.WriteString(string.Empty);
            DefaultValue = new BufferedString(new NativeBuffer(bytes));
        }
        public BufferedString(NativeBuffer buffer) => _start = buffer.Start;
        public BufferedString(void* start) => _start = start;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BufferedString Parse(NativeBuffer buffer) =>  new(buffer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BufferedString Parse(void* start) => new(start);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateSize(void* start) => Unsafe.Read<int>(start);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AsString()
        {
            string v = default!;
            Unsafe.Write(Unsafe.AsPointer(ref v), new IntPtr(Unsafe.Add<byte>(_start, IntPtr.Size * 2)));
            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string(BufferedString value) => value.AsString();

        public override string ToString() => AsString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateStringSize(string? value)
        {
            var byteCount = value is null ? 0 : Encoding.Unicode.GetByteCount(value);

            var size = sizeof(nint)
                + sizeof(nint)
                + sizeof(nint)
                + sizeof(int)
                + byteCount;
            return Math.Max(IntPtr.Size * 3 + sizeof(int), size);
        }
    }
}
