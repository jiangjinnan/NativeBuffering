using System.Runtime.CompilerServices;

namespace NativeBuffering
{
    public unsafe sealed class BufferedObjectWriteContextScope 
    {
        private BufferedObjectWriteContext _writeContext = default!;
        private int _fieldSlot;
        private bool _isSizeCalculateMode;
        public BufferedObjectWriteContextScope(BufferedObjectWriteContext context, int fieldCount)
        {
            _writeContext = context ?? throw new ArgumentNullException(nameof(context));
            _fieldSlot = _writeContext.Position;
            _writeContext.Advance(sizeof(int) * fieldCount);
        }

        public BufferedObjectWriteContextScope() { }
        public void Initialize(BufferedObjectWriteContext writeContext)
        {
            _writeContext = writeContext;
            _fieldSlot = writeContext.Position;
            _isSizeCalculateMode = writeContext.IsSizeCalculateMode;
        }
        public void Release()=> _writeContext = null!;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanagedField<T>(T value) where T : unmanaged => WriteField(value, (c, v) => c.WriteUnmanaged(v), AlignmentCalculator.AlignmentOf<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanagedField<T>(T? value) where T : unmanaged => WriteField(value, (c, v) => c.WriteUnmanaged(v!.Value), AlignmentCalculator.AlignmentOf<T>(), value is null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteStringField(string value) => WriteField(value, (c, v) => c.WriteString(v), IntPtr.Size, string.IsNullOrEmpty(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBinaryField(byte[] value) => WriteField(value, (c, v) => c.WriteBytes(v), IntPtr.Size, value is null || value.Length == 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBinaryField(Memory<byte> value) => WriteField(value, (c, v) => c.WriteBytes(v.Span), IntPtr.Size, value.Length == 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBufferedObjectField<T>(T value) where T : IBufferedObjectSource => WriteField(value, (c, v) => v.Write(c), IntPtr.Size, value is null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteField<T>(T value, Action<BufferedObjectWriteContext, T> writeValue, int alignment, bool isDefault = false)
        {
            if (isDefault || value is null)
            {
                if (!_isSizeCalculateMode)
                {
                    Unsafe.Write(Unsafe.AsPointer(ref _writeContext.Bytes[_fieldSlot]), -1);
                }
            }
            else
            {
                _writeContext.AddPaddingBytes(alignment);
                if (!_writeContext.IsSizeCalculateMode)
                {
                    Unsafe.Write(Unsafe.AsPointer(ref _writeContext.Bytes[_fieldSlot]), _writeContext.Position);
                }
                writeValue(_writeContext, value);
            }
            _fieldSlot += sizeof(int);
        }
    }
}
