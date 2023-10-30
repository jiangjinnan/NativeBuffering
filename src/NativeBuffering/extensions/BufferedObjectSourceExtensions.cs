using System.Buffers;

namespace NativeBuffering
{
    public static class BufferedObjectSourceExtensions
    {
        public static ArraySegment<byte> WriteTo(this IBufferedObjectSource bufferedObjectSource, Func<int, byte[]> bufferProvider)
        {
            var size = bufferedObjectSource.CalculateSize();
            var buffer = bufferProvider(size);
            var context = BufferedObjectWriteContext.Acquire(buffer);
            bufferedObjectSource.Write(context);
            var result = new ArraySegment<byte>(buffer, 0, size);
            BufferedObjectWriteContext.Release(context);
            return result;
        }

        public static async Task WriteToAsync(this IBufferedObjectSource bufferedObjectSource, Stream stream, bool closeStream = false)
        {
            var size = bufferedObjectSource.CalculateSize();
            var bytes = ArrayPool<byte>.Shared.Rent(size);
            var context = BufferedObjectWriteContext.Acquire(bytes);
            try
            {
                bufferedObjectSource.Write(context);
                await stream.WriteAsync(bytes.AsMemory(0, size));
            }
            finally
            {
                BufferedObjectWriteContext.Release(context);
                ArrayPool<byte>.Shared.Return(bytes);
                if (closeStream) stream.Close();
            }
        }

        public static Task WriteToAsync(this IBufferedObjectSource bufferedObjectSource, string filePath)
        => bufferedObjectSource.WriteToAsync(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write), true);
    }
}
