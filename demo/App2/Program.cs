using NativeBuffering;
using System.Diagnostics;

//var foobars = new Foobar[] { new Foobar(1, 2), new Foobar(3, 4) };
//var foobarbaz = new Foobarbaz(foobars, new double[] {1.1,2.2 } );
//var size = foobarbaz.CalculateSize();
//var bytes = new byte[size];
//var context = new BufferedObjectWriteContext(bytes);
//foobarbaz.Write(context);

//var bufferedMessage = FoobarbazBufferedMessage.Parse(new NativeBuffer(bytes));
//Debug.Assert(bufferedMessage.Foobar.Count == 2);
//Debug.Assert(bufferedMessage.Foobar[0].Foo == 1);
//Debug.Assert(bufferedMessage.Foobar[0].Bar == 2);
//Debug.Assert(bufferedMessage.Foobar[1].Foo == 3);
//Debug.Assert(bufferedMessage.Foobar[1].Bar == 4);
//Debug.Assert(bufferedMessage.Baz.Count == 2);
//Debug.Assert(bufferedMessage.Baz[0] == 1.1);
//Debug.Assert(bufferedMessage.Baz[1] == 2.2);


var entity = new Entity
{
    Foo = 123,
    Bar = new UnmanangedStruct(789, 3.14),
    Baz = new byte[] { 1, 2, 3 },
    Qux = "Hello, World!"
};

var byteCount = entity.CalculateSize();
var context = BufferedObjectWriteContext.CreateForSizeCalculation();
entity.Write(context);
var size = context.Position;
Console.WriteLine(byteCount);
Console.WriteLine(size);


//var bytes = new byte[byteCount + 4];
//var context = new BufferedObjectWriteContext(bytes);
//entity.Write(context);
//File.WriteAllBytes(".data", bytes);

//EntityBufferedMessage bufferedMessage;
//BufferOwner? bufferOwner = null;

//try
//{
//    using (var fs = new FileStream(".data", FileMode.Open))
//    {
//        byteCount = (int)fs.Length;
//        bufferOwner = BufferPool.Rent(byteCount);
//        fs.Read(bufferOwner.Bytes, 0, byteCount);
//    }

//    bufferedMessage = BufferedMessage.Create<EntityBufferedMessage>(ref bufferOwner);
//    Debug.Assert(bufferedMessage.Foo == 123);
//    Debug.Assert(bufferedMessage.Bar.X == 789);
//    Debug.Assert(bufferedMessage.Bar.Y == 3.14);
//    Debug.Assert(bufferedMessage.Baz.Length == 3);

//    var byteSpan = bufferedMessage.Baz.AsSpan();
//    Debug.Assert(byteSpan[0] == 1);
//    Debug.Assert(byteSpan[1] == 2);
//    Debug.Assert(byteSpan[2] == 3);

//    Debug.Assert(bufferedMessage.Qux == "Hello, World!");
//}
//finally
//{
//    bufferOwner?.Dispose();
//}

