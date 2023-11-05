using App3;
using BenchmarkDotNet.Running;
using NativeBuffering;
using System.Buffers;
using System.Diagnostics;

//var arraySegment = Record.Instance.WriteTo(ArrayPool<byte>.Shared.Rent);
//ArrayPool<byte>.Shared.Return(arraySegment.Array!);

var summary = BenchmarkRunner.Run(typeof(MyBenchmark));

//var foobar1 = new Foobar { Foo = 1, Bar = 2 };
//var foobar2 = new Foobar { Foo = 3, Bar = 4 };
//var foobar3 = new Foobar { Foo = 5, Bar = 6 };

//var entity = new Entity
//{
//    BoolValue = true,
//    CharValue = 'a',
//    DecimalValue = 1.2m,
//    DoubleValue = 1.2,
//    EnumValue = DateTimeKind.Utc,
//    FloatValue = 1.2f,
//    IntValue = 1,
//    LongValue = 1,
//    SByteValue = 1,
//    ShortValue = 1,
//    TimeSpanValue = TimeSpan.FromDays(1),
//    ULongValue = 1,
//    UIntValue = 1,
//    UShortValue = 1,
//    ByteArrayValue = new byte[] { 1, 2, 3 },
//    ByteValue = 1,
//    MemoryOfByteValue = new byte[] { 1, 2, 3 },
//    StringValue = "Hello World",
//    CollectionValue = new[] { foobar1, foobar2, foobar3 },
//    DictionaryValue = new Dictionary<int, Foobar> { { 1, foobar1 }, { 2, foobar2 }, { 3, foobar3 } },
//};

//await entity.WriteToAsync("entity.bin");
//using (var pooledMessage = await BufferedMessage.LoadAsync<EntityBufferedMessage>("entity.bin"))
//{
//    var message = pooledMessage.BufferedMessage;
//    Debug.Assert(entity.BoolValue == message.BoolValue);
//    Debug.Assert(entity.CharValue == message.CharValue);
//    Debug.Assert(entity.DecimalValue == message.DecimalValue);
//    Debug.Assert(entity.DoubleValue == message.DoubleValue);
//    Debug.Assert(entity.EnumValue == message.EnumValue);
//    Debug.Assert(entity.FloatValue == message.FloatValue);
//    Debug.Assert(entity.IntValue == message.IntValue);
//    Debug.Assert(entity.LongValue == message.LongValue);
//    Debug.Assert(entity.SByteValue == message.SByteValue);
//    Debug.Assert(entity.ShortValue == message.ShortValue);
//    Debug.Assert(entity.TimeSpanValue == message.TimeSpanValue);
//    Debug.Assert(entity.ULongValue == message.ULongValue);
//    Debug.Assert(entity.UIntValue == message.UIntValue);
//    Debug.Assert(entity.UShortValue == message.UShortValue);
//    Debug.Assert(entity.ByteValue == message.ByteValue);
//    Debug.Assert(message.ByteArrayValue.AsSpan().SequenceEqual(entity.ByteArrayValue));
//    Debug.Assert(entity.MemoryOfByteValue.Span.SequenceEqual(message.MemoryOfByteValue.AsSpan()));
//    Debug.Assert(entity.StringValue == message.StringValue);

//    foreach (var fooabr in entity.CollectionValue)
//    {
//        Debug.Assert(message.CollectionValue.Any(it => it!.Value.Foo == fooabr.Foo && it.Value.Bar == fooabr.Bar));
//    }
//    foreach (var (key, value) in entity.DictionaryValue)
//    {
//        Debug.Assert(message.DictionaryValue.Any(it => it.Key == key && it.Value!.Foo == value.Foo && it.Value.Bar == value.Bar));
//    }
//}