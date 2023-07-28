using NativeBuffering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[BufferedMessageSource]
public partial class Entity
{
    public long Foo { get; set; }
    public UnmanangedStruct Bar { get; set; }
    public byte[] Baz { get; set; }
    public string Qux { get; set; }
}
public readonly record struct UnmanangedStruct(int X, double Y);

