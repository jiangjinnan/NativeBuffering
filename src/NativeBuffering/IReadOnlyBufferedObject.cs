namespace NativeBuffering
{
public interface IReadOnlyBufferedObject<T> where T: IReadOnlyBufferedObject<T>
{
    static abstract T Parse(NativeBuffer buffer);
}
}

