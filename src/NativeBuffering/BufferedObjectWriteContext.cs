using System.Runtime.CompilerServices;
using System.Text;

namespace NativeBuffering
{
    public unsafe sealed class BufferedObjectWriteContext
    {
        private static readonly IntPtr _stringTypeHandle = typeof(string).TypeHandle.Value;
        private int _position = 0;
        public bool IsSizeCalculateMode { get; } = false;
        public byte[] Bytes { get; }
        public int Position => _position;
        public void* PositionAsPointer => Unsafe.AsPointer(ref Bytes[_position]);
        public BufferedObjectWriteContext(byte[] buffer, bool isSizeCalculateMode = false)
        {
            Bytes = buffer;
            IsSizeCalculateMode = isSizeCalculateMode;
        }
        public static BufferedObjectWriteContext CreateForSizeCalculation() => new(Array.Empty<byte>(), true);
        public void Advance(int step) => _position += step;
        public unsafe void WriteUnmanaged<T>(T value) where T : unmanaged
        {
            if (!IsSizeCalculateMode)
            {
                Unsafe.Write(Unsafe.AsPointer(ref Bytes[_position]), value);
            }
            _position += sizeof(T);
        }
        public void WriteString(string value)
        {
            if(value is null) throw new ArgumentNullException(nameof(value));

            var size = BufferedString.CalculateStringSize(value);

            if (!IsSizeCalculateMode)
            {
                Unsafe.Write(Unsafe.AsPointer(ref Bytes[_position]), size);
            }
            _position += sizeof(int) + (IntPtr.Size - sizeof(int)) + IntPtr.Size;

            if (!IsSizeCalculateMode)
            {
                Unsafe.Write(Unsafe.AsPointer(ref Bytes[_position]), _stringTypeHandle);
            }
            _position += IntPtr.Size;

            if (!IsSizeCalculateMode)
            {
                Unsafe.Write(Unsafe.AsPointer(ref Bytes[_position]), value?.Length ?? 0);
            }
            _position += sizeof(int);

            if (!string.IsNullOrEmpty(value))
            {
                var bytes = Encoding.Unicode.GetBytes(value);
                if (!IsSizeCalculateMode)
                {
                    Unsafe.CopyBlock(ref Bytes[_position], ref bytes[0], (uint)bytes.Length);
                }

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
            if (!IsSizeCalculateMode)
            {
                Unsafe.Write(Unsafe.AsPointer(ref Bytes[_position]), bytes.Length);
            }
            _position += sizeof(int);
            if (!IsSizeCalculateMode && bytes.Length > 0)
            {
                Unsafe.CopyBlock(ref Bytes[_position], ref bytes[0], (uint)bytes.Length);
            }
            _position += bytes.Length;
        }
        public int AddPaddingBytes(int alignment)
        {
            var paddingBytes = _position % alignment;
            if (paddingBytes != 0)
            {
                paddingBytes = alignment - paddingBytes;                
                _position += paddingBytes;
            }
            return _position;
        }

        public void EnsureAlignment(int alignment)
        {
           if(_position % alignment != 0) throw new InvalidOperationException($"Position is not aligned to {alignment} bytes");
        }
    }
}