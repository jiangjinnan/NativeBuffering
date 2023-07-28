namespace NativeBuffering
{
    public static class BufferedMessage
    {
        public static T Create<T>(ref BufferOwner byteArrayOwner, int offset = 0) where T : IReadOnlyBufferedObject<T>
        {
            return T.Parse(new NativeBuffer(byteArrayOwner.Bytes, offset));
        }
    }
}
