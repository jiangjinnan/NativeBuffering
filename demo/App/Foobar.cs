using NativeBuffering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    [BufferedMessageSource]
    public partial class Foobar
    {
        public Foobar(int foo, string bar)
        {
            Foo = foo;
            Bar = bar;
        }

        public int Foo { get; }
        public string Bar { get; }
    }
}
