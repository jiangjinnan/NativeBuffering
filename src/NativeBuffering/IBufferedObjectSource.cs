namespace NativeBuffering
{
    public interface IBufferedObjectSource
    {
        int CalculateSize();
        void Write(BufferedObjectWriteContext context);
    }
}
