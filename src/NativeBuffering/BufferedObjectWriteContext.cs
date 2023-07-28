using System.Runtime.CompilerServices;
using System.Text;

namespace NativeBuffering
{
    public unsafe sealed class BufferedObjectWriteContext
    {
        private static readonly IntPtr _stringTypeHandle = typeof(string).TypeHandle.Value;
        private int _position = 0;
        public byte[] Bytes { get; }
        public int Position => _position;
        public void* PositionAsPointer => Unsafe.AsPointer(ref Bytes[_position]);
        public BufferedObjectWriteContext(byte[] buffer) => Bytes = buffer;
        public void Advance(int step) => _position += step;
        public unsafe void WriteUnmanaged<T>(T value) where T : unmanaged
        {
            Unsafe.Write(Unsafe.AsPointer(ref Bytes[_position]), value);
            _position += sizeof(T);
        }
        public void WriteString(string value)
        {
            var size = Utilities.CalculateStringSize(value);

            Unsafe.Write(Unsafe.AsPointer(ref Bytes[_position]), size);
            _position += sizeof(int) + sizeof(nint);

            Unsafe.Write(Unsafe.AsPointer(ref Bytes[_position]), _stringTypeHandle);
            _position += sizeof(nint);

            Unsafe.Write(Unsafe.AsPointer(ref Bytes[_position]), value?.Length ?? 0);
            _position += sizeof(int);

            if (!string.IsNullOrEmpty(value))
            {
                var bytes = Encoding.Unicode.GetBytes(value);
                Unsafe.CopyBlock(ref Bytes[_position], ref bytes[0], (uint)bytes.Length);
                if (IntPtr.Size == 4 || bytes.Length >= 4)
                {
                    _position += bytes.Length;
                }
                else
                {
                    _position += 4;
                }
            }
            else if (IntPtr.Size == 8)
            {
                _position += 4;
            }
        }
        public void WriteBytes(Span<byte> bytes)
        {
            Unsafe.Write(Unsafe.AsPointer(ref Bytes[_position]), bytes.Length);
            _position += sizeof(int);

            Unsafe.CopyBlock(ref Bytes[_position], ref bytes[0], (uint)bytes.Length);
            _position += bytes.Length;
        }
        public void WriteByteCount(int count)=> WriteUnmanaged(count);
    }
}