using Microsoft.Extensions.ObjectPool;
using System.Runtime.CompilerServices;
using System.Text;

namespace NativeBuffering
{
    public unsafe sealed class BufferedObjectWriteContext
    {
        private static readonly ObjectPool<BufferedObjectWriteContext> _pool = new DefaultObjectPoolProvider().Create<BufferedObjectWriteContext>(new BufferedObjectWriteContextPooledObjectPolicy());
        private static readonly IntPtr _stringTypeHandle = typeof(string).TypeHandle.Value;

        private byte[] _bytes = null!;
        private int _position = 0;
        public bool IsSizeCalculateMode { get; private set; }
        public byte[] Bytes => _bytes;

        public int Position => _position;
        public void* PositionAsPointer => Unsafe.AsPointer(ref Bytes[_position]);
        private BufferedObjectWriteContext() { }
        public BufferedObjectWriteContext Initialize(byte[] buffer, bool isSizeCalculateMode)
        {
            _bytes = buffer;
            IsSizeCalculateMode = isSizeCalculateMode;
            _position = 0;
            return this;
        }

        public void Release()=> _bytes = null!;
        //public static BufferedObjectWriteContext CreateForSizeCalculation() => new(Array.Empty<byte>(), true);
        public void Advance(int step) => _position += step;
        public unsafe void WriteUnmanaged<T>(T value) where T : unmanaged
        {
            if (!IsSizeCalculateMode)
            {
                Unsafe.Write(Unsafe.AsPointer(ref Bytes[_position]), value);
            }
            _position += sizeof(T);
        }

        //private readonly int _sizeByteCount = sizeof(int) + (IntPtr.Size - sizeof(int));
        public void WriteString(string value)
        {
            if(value is null) throw new ArgumentNullException(nameof(value));
            var size = BufferedString.CalculateStringSize(value);
            if (!IsSizeCalculateMode)
            {
                //Size + Padding
                Unsafe.Write(Unsafe.AsPointer(ref Bytes[_position]), size);
                _position += IntPtr.Size;

                var address = *(nint*)Unsafe.AsPointer(ref value) - nint.Size;
                var pointer = address.ToPointer();
                Unsafe.CopyBlock(PositionAsPointer, pointer, (uint)size);
                _position += size - IntPtr.Size;
                return;
            }
            _position += size;
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
        public static BufferedObjectWriteContext Acquire(byte[] buffer) => _pool.Get().Initialize(buffer, false);
        public static BufferedObjectWriteContext AcquireForSizeCalculation() => _pool.Get().Initialize(null!, true);
        public static void Release(BufferedObjectWriteContext writeContext)=> _pool.Return(writeContext);
        public static BufferedObjectWriteContext Create(byte[] buffer)=> new BufferedObjectWriteContext().Initialize(buffer, false);
        private sealed class BufferedObjectWriteContextPooledObjectPolicy : PooledObjectPolicy<BufferedObjectWriteContext>
        {
            public override BufferedObjectWriteContext Create() => new ();
            public override bool Return(BufferedObjectWriteContext obj)
            {
                obj.Release();
                return true;
            }
        }
    }
}