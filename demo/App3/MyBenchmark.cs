using BenchmarkDotNet.Attributes;
using NativeBuffering;
using System.Buffers;
using System.Text;
using System.Text.Json;

namespace App3
{
    [MemoryDiagnoser]
    [Config(typeof(AntiVirusFriendlyConfig))]
    public class MyBenchmark
    {
        private Entity _entity = default!;

        [GlobalSetup]
        public void Setup()
        {
            var foobar1 = new Foobar { Foo = 1, Bar = 2 };
            var foobar2 = new Foobar { Foo = 3, Bar = 4 };
            var foobar3 = new Foobar { Foo = 5, Bar = 6 };

            _entity = new Entity
            {
                BoolValue = true,
                CharValue = 'a',
                DecimalValue = 1.2m,
                DoubleValue = 1.2,
                EnumValue = DateTimeKind.Utc,
                FloatValue = 1.2f,
                IntValue = 1,
                LongValue = 1,
                SByteValue = 1,
                ShortValue = 1,
                TimeSpanValue = TimeSpan.FromDays(1),
                ULongValue = 1,
                UIntValue = 1,
                UShortValue = 1,
                ByteArrayValue = new byte[] { 1, 2, 3 },
                ByteValue = 1,
                //MemoryOfByteValue = new byte[] { 1, 2, 3 },
                StringValue = "Hello World",
                CollectionValue = new[] { foobar1, foobar2, foobar3 },
                //DictionaryValue = new Dictionary<int, Foobar> { { 1, foobar1 }, { 2, foobar2 }, { 3, foobar3 } },
            };
        }

        private Func<int, byte[]> _bufferFactory = ArrayPool<byte>.Shared.Rent;
        [Benchmark]
        public byte[] SerializeAsJson()=> Encoding.UTF8.GetBytes(JsonSerializer.Serialize(_entity!));

        [Benchmark]
        public void SerializeNativeBuffering()
        {
            var arraySegment = _entity.WriteTo(_bufferFactory);
            ArrayPool<byte>.Shared.Return(arraySegment.Array!);

            //var size = _entity.CalculateSize();
            //var bytes = ArrayPool<byte>.Shared.Rent(size);
            //var context = BufferedObjectWriteContext.Acquire(bytes);
            //_entity.Write(context);
            //BufferedObjectWriteContext.Release(context);
            //ArrayPool<byte>.Shared.Return(bytes);
        }
    }
}
