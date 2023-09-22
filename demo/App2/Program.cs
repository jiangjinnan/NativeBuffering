using NativeBuffering;
using System.Diagnostics;

var entity = new Entity
{
    //Primitive = 123,
    //Unmanaged = new Foobar(123, 789),
    //Bytes = Enumerable.Range(1, 128).Select(_ => byte.MaxValue).ToArray(),
    //String = "abc",
    //BufferedObject = new Foobarbaz(new Foobar(111, 222), "xyz"),

    PrimitiveList = new int[] { 1, 2, 3 },
    UnmanagedList = new Foobar[] { new Foobar(1, 2), new Foobar(3, 4) },
    StringList = new string[] { "a", "b", "c" },
    BufferedObjectList = new Foobarbaz[] { new Foobarbaz(new Foobar(1, 2), "a"), new Foobarbaz(new Foobar(3, 4), "b") }
};

using (var pooledMessage = entity.AsBufferedMessage<EntityBufferedMessage>())
{
    var bufferedMessage = pooledMessage.BufferedMessage;
    Debug.Assert(bufferedMessage.PrimitiveList.Count == 3);
    Debug.Assert(bufferedMessage.PrimitiveList[0] == 1);
    Debug.Assert(bufferedMessage.PrimitiveList[1] == 2);
    Debug.Assert(bufferedMessage.PrimitiveList[2] == 3);

    Debug.Assert(bufferedMessage.UnmanagedList.Count == 2);
    Debug.Assert(bufferedMessage.UnmanagedList[0].Foo == 1);
    Debug.Assert(bufferedMessage.UnmanagedList[0].Bar == 2);
    Debug.Assert(bufferedMessage.UnmanagedList[1].Foo == 3);
    Debug.Assert(bufferedMessage.UnmanagedList[1].Bar == 4);

    Debug.Assert(bufferedMessage.StringList.Count == 3);
    Debug.Assert(bufferedMessage.StringList[0] == "a");
    Debug.Assert(bufferedMessage.StringList[1] == "b");
    Debug.Assert(bufferedMessage.StringList[2] == "c");

    Debug.Assert(bufferedMessage.BufferedObjectList.Count == 2);
    Debug.Assert(bufferedMessage.BufferedObjectList[0]!.Value.Foobar.Foo == 1);
    Debug.Assert(bufferedMessage.BufferedObjectList[0]!.Value.Foobar.Bar == 2);
    Debug.Assert(bufferedMessage.BufferedObjectList[0]!.Value.Baz == "a");
    Debug.Assert(bufferedMessage.BufferedObjectList[1]!.Value.Foobar.Foo == 3);
    Debug.Assert(bufferedMessage.BufferedObjectList[1]!.Value.Foobar.Bar == 4);
    Debug.Assert(bufferedMessage.BufferedObjectList[1]!.Value.Baz == "b");
}