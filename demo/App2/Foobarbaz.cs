using NativeBuffering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[BufferedMessageSource]
public partial class Foobarbaz
{
    public Foobar[] Foobar { get; }
    public double[] Baz { get; }
    public Foobarbaz(Foobar[] foobar, double[] baz)
    {
        Foobar = foobar;
        Baz = baz;
    }
}

[BufferedMessageSource]
public partial class Foobar
{
    public int Foo { get; }
    public long Bar { get; }
    public Foobar(int foo, long bar)
    {
        Foo = foo;
        Bar = bar;
    }
}
