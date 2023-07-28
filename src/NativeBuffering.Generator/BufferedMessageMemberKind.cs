namespace NativeBuffering.Generator
{
    public enum BufferedMessageMemberKind
    {
        Primitive,
        Unmanaged,
        String,
        Message,
        Binary,

        UnmanagedUnmanagedDictionary,
        UnmanagedStringDictionary,
        UnmanagedMessageDictionary,
        UnmanagedBinaryDictionary,

        StringUnmanagedDictionary,
        StringStringDictionary,
        StringMessageDictionary,
        StringBinaryDictionary,

        UnmanagedCollection,
        StringCollection,
        MessageCollection,
        BinaryCollection
    }
}
