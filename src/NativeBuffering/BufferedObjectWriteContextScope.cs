using System.Runtime.CompilerServices;

namespace NativeBuffering
{
    public unsafe sealed class BufferedObjectWriteContextScope : IDisposable
    {
        private readonly BufferedObjectWriteContext _context;
        private readonly Queue<Action<BufferedObjectWriteContext>> _pendingWriteActions = new();
        public BufferedObjectWriteContextScope(BufferedObjectWriteContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));
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

        internal void WriteField<T>(T value, Action<BufferedObjectWriteContext, T> writeValue, int alignment, bool isDefault = false)
        {
            var fieldSlot = _context.Position;
            _context.Advance(sizeof(int));
            _pendingWriteActions.Enqueue(context =>
            {
                if (isDefault || value is null )
                {
                    if (!context.IsSizeCalculateMode)
                    {
                        Unsafe.Write(Unsafe.AsPointer(ref context.Bytes[fieldSlot]), -1);
                    }
                    return;
                }
                context.AddPaddingBytes(alignment);
                if (!context.IsSizeCalculateMode)
                {
                    Unsafe.Write(Unsafe.AsPointer(ref context.Bytes[fieldSlot]), context.Position);
                }
                writeValue(context, value);
            });
        }
        public void Dispose()
        {
            foreach (var write in _pendingWriteActions)
            {
                write(_context);
            }
        }
    }
}
