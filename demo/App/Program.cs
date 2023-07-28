using App;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run(typeof(int));

//using App;
//using NativeBuffering;
//using System.Diagnostics;
//using System.Text;
//using System.Text.Json;
//using System.Buffers;

//var entity = new Entity
//{
//    Pritimive = 123,
//    Unmanaged = new Pointer(1, 2),
//    String = "Hello World!",
//    Binary = new byte[] { 1, 2, 3 },
//    BufferedMessage = new Foobar(123, "Hello World!"),

//    UnmanagedCollection = new Pointer[] { new Pointer(1, 2), new Pointer(3, 4) },
//    StringCollection = new string[] { "Hello", "World" },
//    BinaryCollection = new byte[][] { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 } },
//    BufferedMessageCollection = new Foobar[] { new Foobar(1, "a"), new Foobar(2, "b") },

//    UnmanagedUnmanagedDictionary = new Dictionary<long, Pointer> { { 1, new Pointer(1, 2) }, { 2, new Pointer(3, 4) } },
//    UnmanagedStringDictionary = new Dictionary<long, string> { { 1, "Hello" }, { 2, "World" } },
//    UnmanagedBinaryDictionary = new Dictionary<long, byte[]> { { 1, new byte[] { 1, 2, 3 } }, { 2, new byte[] { 4, 5, 6 } } },
//    UnmanagedBufferedMessageDictionary = new Dictionary<long, Foobar> { { 1, new Foobar(1, "a") }, { 2, new Foobar(2, "b") } },

//    StringUnmanagedDictionary = new Dictionary<string, Pointer> { { "a", new Pointer(1, 2) }, { "b", new Pointer(3, 4) } },
//    StringStringDictionary = new Dictionary<string, string> { { "a", "Hello" }, { "b", "World" } },
//    StringBinaryDictionary = new Dictionary<string, byte[]> { { "a", new byte[] { 1, 2, 3 } }, { "b", new byte[] { 4, 5, 6 } } },
//    StringBufferedMessageDictionary = new Dictionary<string, Foobar> { { "a", new Foobar(1, "a") }, { "b", new Foobar(2, "b") } },
//};

//var json = JsonSerializer.Serialize(entity);
//var jsonBytes = Encoding.UTF8.GetBytes(json);



//var bytes = new byte[entity.CalculateSize()];
//var context = new BufferedObjectWriteContext(bytes);
//entity.Write(context);

//var messsage = EntityBufferedMessage.Parse(new NativeBuffer(bytes, 0));

//using (var owner = BufferedMessage.Create<EntityBufferedMessage>(bytes))
//{
//    messsage = owner.BufferedMessage;

//    Debug.Assert(messsage.Pritimive == 123);
//    Debug.Assert(messsage.Unmanaged == new Pointer(1, 2));
//    Debug.Assert(messsage.String == "Hello World!");
//    Debug.Assert(messsage.Binary.AsSpan().SequenceEqual(new byte[] { 1, 2, 3 }));
//    Debug.Assert(messsage.BufferedMessage.Foo == 123);
//    Debug.Assert(messsage.BufferedMessage.Bar == "Hello World!");

//    Debug.Assert(messsage.UnmanagedCollection.Count == 2);
//    Debug.Assert(messsage.UnmanagedCollection[0] == new Pointer(1, 2));
//    Debug.Assert(messsage.UnmanagedCollection[1] == new Pointer(3, 4));

//    Debug.Assert(messsage.StringCollection.Count == 2);
//    Debug.Assert(messsage.StringCollection[0] == "Hello");
//    Debug.Assert(messsage.StringCollection[1] == "World");

//    Debug.Assert(messsage.BinaryCollection.Count == 2);
//    Debug.Assert(messsage.BinaryCollection[0].AsSpan().SequenceEqual(new byte[] { 1, 2, 3 }));
//    Debug.Assert(messsage.BinaryCollection[1].AsSpan().SequenceEqual(new byte[] { 4, 5, 6 }));

//    Debug.Assert(messsage.BufferedMessageCollection.Count == 2);
//    Debug.Assert(messsage.BufferedMessageCollection[0].Foo == 1);
//    Debug.Assert(messsage.BufferedMessageCollection[0].Bar == "a");
//    Debug.Assert(messsage.BufferedMessageCollection[1].Foo == 2);
//    Debug.Assert(messsage.BufferedMessageCollection[1].Bar == "b");

//    Debug.Assert(messsage.UnmanagedUnmanagedDictionary.Count == 2);
//    Debug.Assert(messsage.UnmanagedUnmanagedDictionary[1] == new Pointer(1, 2));
//    Debug.Assert(messsage.UnmanagedUnmanagedDictionary[2] == new Pointer(3, 4));

//    Debug.Assert(messsage.UnmanagedStringDictionary.Count == 2);
//    Debug.Assert(messsage.UnmanagedStringDictionary[1] == "Hello");
//    Debug.Assert(messsage.UnmanagedStringDictionary[2] == "World");

//    Debug.Assert(messsage.UnmanagedBinaryDictionary.Count == 2);
//    Debug.Assert(messsage.UnmanagedBinaryDictionary[1].AsSpan().SequenceEqual(new byte[] { 1, 2, 3 }));
//    Debug.Assert(messsage.UnmanagedBinaryDictionary[2].AsSpan().SequenceEqual(new byte[] { 4, 5, 6 }));

//    Debug.Assert(messsage.UnmanagedBufferedMessageDictionary.Count == 2);
//    Debug.Assert(messsage.UnmanagedBufferedMessageDictionary[1].Foo == 1);
//    Debug.Assert(messsage.UnmanagedBufferedMessageDictionary[1].Bar == "a");

//    Debug.Assert(messsage.StringUnmanagedDictionary.Count == 2);
//    Debug.Assert(messsage.StringUnmanagedDictionary["a"] == new Pointer(1, 2));
//    Debug.Assert(messsage.StringUnmanagedDictionary["b"] == new Pointer(3, 4));

//    Debug.Assert(messsage.StringStringDictionary.Count == 2);
//    Debug.Assert(messsage.StringStringDictionary["a"] == "Hello");
//    Debug.Assert(messsage.StringStringDictionary["b"] == "World");

//    Debug.Assert(messsage.StringBinaryDictionary.Count == 2);
//    Debug.Assert(messsage.StringBinaryDictionary["a"].AsSpan().SequenceEqual(new byte[] { 1, 2, 3 }));
//    Debug.Assert(messsage.StringBinaryDictionary["b"].AsSpan().SequenceEqual(new byte[] { 4, 5, 6 }));

//    Debug.Assert(messsage.StringBufferedMessageDictionary.Count == 2);
//    Debug.Assert(messsage.StringBufferedMessageDictionary["a"].Foo == 1);
//    Debug.Assert(messsage.StringBufferedMessageDictionary["a"].Bar == "a");
//}

//using (var owner = BufferedMessage.Create<EntityBufferedMessage>(bytes))
//{ }
