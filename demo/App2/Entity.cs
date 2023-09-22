#nullable disable
using NativeBuffering;

[BufferedMessageSource]
public partial class Entity
{
    public int Primitive { get; set; }
    public Foobar Unmanaged { get; set; }
    public byte[] Bytes { get; set; }
    public string String { get; set; }
    public Foobarbaz BufferedObject { get; set; }

    public IEnumerable<int> PrimitiveList { get; set; }
    public IEnumerable<Foobar> UnmanagedList { get; set; }
    public IEnumerable<string> StringList { get; set; }
    public IEnumerable<Foobarbaz> BufferedObjectList { get; set; }
}

[BufferedMessageSource]
public partial class Foobarbaz
{
    public Foobarbaz(Foobar foobar, string baz)
    {
        Foobar = foobar;
        Baz = baz;
    }
    public Foobar Foobar { get; }
    public string Baz { get; }
}
public readonly record struct Foobar(int Foo, long Bar);

#nullable restore
