using NativeBuffering;

[BufferedMessageSource]
public partial class Record
{
    public Foobarbazqux[] Data { get; set; } = default!;
    public static Record Instance => new Record { Data = Enumerable.Range(1, 100).Select(_ => new Foobarbazqux(new Foobarbaz(new Foobar(111, 222), 1.234f), 3.14d)).ToArray() };
}

public readonly record struct Foobar(int Foo, long Bar);
public readonly record struct Foobarbaz(Foobar Foobar, float Baz);
public readonly record struct Foobarbazqux(Foobarbaz Foobarbaz, double Qux);