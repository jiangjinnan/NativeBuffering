using System.Runtime.CompilerServices;

namespace NativeBuffering
{
    public unsafe sealed class BufferedObjectWriteContextScope : IDisposable
    {
        private readonly BufferedObjectWriteContext _context;
        private readonly Queue<Action<BufferedObjectWriteContext>> _pendingWriteActions = new();
        public BufferedObjectWriteContextScope(BufferedObjectWriteContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));
        public void WriteUnmanagedField<T>(T value) where T : unmanaged => WriteField(value, (c, v) => c.WriteUnmanaged(v));
        public void WriteStringField(string value) => WriteField(value, (c, v) => c.WriteString(v));
        public void WriteBinaryField(byte[] value) => WriteField(value, (c, v) => c.WriteBytes(v));
        public void WriteBinaryField(Memory<byte> value) => WriteField(value, (c, v) => c.WriteBytes(v.Span));
        public void WriteBufferedObjectField<T>(T value) where T : IBufferedObjectSource => WriteField(value, (c, v) => v.Write(c));
        public void WriteField<T>(T value, Action<BufferedObjectWriteContext, T> writeValue)
        {
            var fieldSlot = _context.Position;
            _context.Advance(sizeof(int));
            _pendingWriteActions.Enqueue(context =>
            {
                Unsafe.Write(Unsafe.AsPointer(ref context.Bytes[fieldSlot]), context.Position);
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
