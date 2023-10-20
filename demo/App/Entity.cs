using NativeBuffering;

namespace App
{
    [BufferedMessageSource]
    public partial class Entity
    {
        public int Pritimive { get; set; }
        public Pointer Unmanaged { get; set; }
        public string String { get; set; } = default!;
        public byte[] Binary { get; set; } = default!;
        public Foobar BufferedMessage { get; set; } = default!;

        public Pointer[] UnmanagedCollection { get; set; } = default!;
        public string[] StringCollection { get; set; } = default!;
        public byte[][] BinaryCollection { get; set; } = default!;
        public Foobar[] BufferedMessageCollection { get; set; } = default!;

        public Dictionary<long, Pointer> UnmanagedUnmanagedDictionary { get; set; } = default!;
        //public Dictionary<long, string> UnmanagedStringDictionary { get; set; } = default!;
        //public Dictionary<long, byte[]> UnmanagedBinaryDictionary { get; set; } = default!;
        //public Dictionary<long, Foobar> UnmanagedBufferedMessageDictionary { get; set; } = default!;

        //public Dictionary<string, Pointer> StringUnmanagedDictionary { get; set; } = default!;
        //public Dictionary<string, string> StringStringDictionary { get; set; } = default!;
        //public Dictionary<string, byte[]> StringBinaryDictionary { get; set; } = default!;
        //public Dictionary<string, Foobar> StringBufferedMessageDictionary { get; set; } = default!;
    }
}
