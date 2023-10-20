using BenchmarkDotNet.Attributes;
using NativeBuffering;
using System.Buffers;
using System.Text.Json;

namespace App
{
    [MemoryDiagnoser]
    [Config(typeof(AntiVirusFriendlyConfig))]
    public class MyBenchmark
    {
        private string? _json;
        private static byte[]? _bytes;

        private Entity _entity;
        private EntityBufferedMessage _bufferedMessage;

        [GlobalSetup]
        public void Setup()
        {
            var entity = new Entity
            {
                Pritimive = 123,
                Unmanaged = new Pointer(1, 2),
                String = "Hello World!",
                Binary = new byte[] { 1, 2, 3 },
                BufferedMessage = new Foobar(123, "Hello World!"),

                UnmanagedCollection = new Pointer[] { new Pointer(1, 2), new Pointer(3, 4) },
                StringCollection = new string[] { "Hello", "World" },
                BinaryCollection = new byte[][] { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 } },
                BufferedMessageCollection = new Foobar[] { new Foobar(1, "a"), new Foobar(2, "b") },

                UnmanagedUnmanagedDictionary = new Dictionary<long, Pointer> { { 1, new Pointer(1, 2) }, { 2, new Pointer(3, 4) } },
                //UnmanagedStringDictionary = new Dictionary<long, string> { { 1, "Hello" }, { 2, "World" } },
                //UnmanagedBinaryDictionary = new Dictionary<long, byte[]> { { 1, new byte[] { 1, 2, 3 } }, { 2, new byte[] { 4, 5, 6 } } },
                //UnmanagedBufferedMessageDictionary = new Dictionary<long, Foobar> { { 1, new Foobar(1, "a") }, { 2, new Foobar(2, "b") } },

                //StringUnmanagedDictionary = new Dictionary<string, Pointer> { { "a", new Pointer(1, 2) }, { "b", new Pointer(3, 4) } },
                //StringStringDictionary = new Dictionary<string, string> { { "a", "Hello" }, { "b", "World" } },
                //StringBinaryDictionary = new Dictionary<string, byte[]> { { "a", new byte[] { 1, 2, 3 } }, { "b", new byte[] { 4, 5, 6 } } },
                //StringBufferedMessageDictionary = new Dictionary<string, Foobar> { { "a", new Foobar(1, "a") }, { "b", new Foobar(2, "b") } },
            };

            _json = System.Text.Json.JsonSerializer.Serialize(entity);
            _bytes = GC.AllocateUninitializedArray<byte>(entity.CalculateSize(), true);
            var context =  BufferedObjectWriteContext.Create(_bytes);
            entity.Write(context);

            _entity = entity;
            _bufferedMessage = EntityBufferedMessage.Parse(new NativeBuffer(_bytes, 0));

            //using (BufferedMessage.Create<EntityBufferedMessage>(_bytes!)) ;
            //var array = _pool.Rent(_bytes!.Length);
            //_pool.Return(array);
        }

        [Benchmark]
        public string SerializeAsJson()=> JsonSerializer.Serialize(_entity!);

        [Benchmark]
        public void SerializeNativeBuffering()
        {
            var size = _entity.CalculateSize();
            var bytes = ArrayPool<byte>.Shared.Rent(size);
            var context = BufferedObjectWriteContext.Acquire(bytes);
            _entity.Write(context);
            BufferedObjectWriteContext.Release(context);
            ArrayPool<byte>.Shared.Return(bytes);
        }

        //[Benchmark]
        public string UseEntity()
        {
            return JsonSerializer.Serialize(_entity!);
            //var entity = JsonSerializer.Deserialize<Entity>(_json!)!;
            //var entity = _entity!;
            //Process(entity.Pritimive);
            //Process(entity.Unmanaged);
            //Process(entity.String);
            //Process(entity.Binary);
            //Process(entity.BufferedMessage);

            //foreach (var item in entity.UnmanagedCollection)
            //{
            //    Process(item);
            //}
            //foreach (var item in entity.StringCollection)
            //{
            //    Process(item);
            //}
            //foreach (var item in entity.BinaryCollection)
            //{
            //    Process(item);
            //}
            //foreach (var item in entity.BufferedMessageCollection)
            //{
            //    Process(item.Foo);
            //    Process(item.Bar);
            //}

            //foreach (var item in entity.UnmanagedUnmanagedDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value);
            //}

            //foreach (var item in entity.UnmanagedStringDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value);
            //}

            //foreach (var item in entity.UnmanagedBinaryDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value);
            //}

            //foreach (var item in entity.UnmanagedBufferedMessageDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value.Foo);
            //    Process(item.Value.Bar);
            //}

            //foreach (var item in entity.StringUnmanagedDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value);
            //}

            //foreach (var item in entity.StringStringDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value);
            //}

            //foreach (var item in entity.StringBinaryDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value);
            //}

            //foreach (var item in entity.StringBufferedMessageDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value.Foo);
            //    Process(item.Value.Bar);
            //}
        }

        //[Benchmark]
        public void UseEntityBufferedMessageFor()
        {
            var size = _entity.CalculateSize();
            var bytes = ArrayPool<byte>.Shared.Rent(size);
            var context = BufferedObjectWriteContext.Acquire(bytes);
            _entity.Write(context);
            BufferedObjectWriteContext.Release(context);
            ArrayPool<byte>.Shared.Return(bytes);


            using (var pooledMessage = _entity!.AsBufferedMessage<EntityBufferedMessage>())
            {
                Process(pooledMessage.BufferedMessage.Pritimive);
            }
            //var entity = _bufferedMessage!;
            //Process(entity.Pritimive);
            //Process(entity.Unmanaged);
            //Process(entity.String);
            //Process(entity.Binary);
            //Process(entity.BufferedMessage);

            //var unmanagedCollection = entity.UnmanagedCollection;
            //for (var index = 0; index < unmanagedCollection.Count; index++)
            //{
            //    Process(unmanagedCollection[index]);
            //}

            //var stringCollection = entity.StringCollection;
            //for (var index = 0; index < stringCollection.Count; index++)
            //{
            //    Process(stringCollection[index]);
            //}

            //var binaryCollection = entity.BinaryCollection;
            //for (var index = 0; index < binaryCollection.Count; index++)
            //{
            //    Process(binaryCollection[index]);
            //}

            //var bufferedMessageCollection = entity.BufferedMessageCollection;
            //for (var index = 0; index < bufferedMessageCollection.Count; index++)
            //{
            //    Process(bufferedMessageCollection[index].Foo);
            //    Process(bufferedMessageCollection[index].Bar);
            //}

            //var unmanagedUnmanagedDictionary = entity.UnmanagedUnmanagedDictionary;
            //Process(unmanagedUnmanagedDictionary[1]);
            //Process(unmanagedUnmanagedDictionary[2]);

            //var unmanagedStringDictionary = entity.UnmanagedStringDictionary;
            //Process(unmanagedStringDictionary[1]);
            //Process(unmanagedStringDictionary[2]);

            //var unmanagedBinaryDictionary = entity.UnmanagedBinaryDictionary;
            //Process(unmanagedBinaryDictionary[1]);
            //Process(unmanagedBinaryDictionary[2]);

            //var unmanagedBufferedMessageDictionary = entity.UnmanagedBufferedMessageDictionary;
            //Process(unmanagedBufferedMessageDictionary[1].Foo);
            //Process(unmanagedBufferedMessageDictionary[1].Bar);
            //Process(unmanagedBufferedMessageDictionary[2].Foo);
            //Process(unmanagedBufferedMessageDictionary[2].Bar);

            //var stringUnmanagedDictionary = entity.StringUnmanagedDictionary;
            //Process(stringUnmanagedDictionary["a"]);
            //Process(stringUnmanagedDictionary["b"]);

            //var stringStringDictionary = entity.StringStringDictionary;
            //Process(stringStringDictionary["a"]);
            //Process(stringStringDictionary["b"]);

            //var stringBufferedMessageDictionary = entity.StringBufferedMessageDictionary;
            //Process(stringBufferedMessageDictionary["a"].Foo);
            //Process(stringBufferedMessageDictionary["a"].Bar);
            //Process(stringBufferedMessageDictionary["b"].Foo);
            //Process(stringBufferedMessageDictionary["b"].Bar);

            //var stringBinaryDictionary = entity.StringBinaryDictionary;
            //Process(stringBinaryDictionary["a"]);
            //Process(stringBinaryDictionary["b"]);
        }

        //[Benchmark]
        public void UseEntityBufferedMessageForeach()
        {
            //var entity = JsonSerializer.Deserialize<Entity>(_json!)!;
            //var entity = _bufferedMessage!;
            //Process(entity.Pritimive);
            //Process(entity.Unmanaged);
            //Process(entity.String);
            //Process(entity.Binary);
            //Process(entity.BufferedMessage);

            //foreach (var item in entity.UnmanagedCollection)
            //{
            //    Process(item);
            //}
            //foreach (var item in entity.StringCollection)
            //{
            //    Process(item);
            //}
            //foreach (var item in entity.BinaryCollection)
            //{
            //    Process(item);
            //}
            //foreach (var item in entity.BufferedMessageCollection)
            //{
            //    Process(item.Foo);
            //    Process(item.Bar);
            //}

            //foreach (var item in entity.UnmanagedUnmanagedDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value);
            //}

            //foreach (var item in entity.UnmanagedStringDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value);
            //}

            //foreach (var item in entity.UnmanagedBinaryDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value);
            //}

            //foreach (var item in entity.UnmanagedBufferedMessageDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value.Foo);
            //    Process(item.Value.Bar);
            //}

            //foreach (var item in entity.StringUnmanagedDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value);
            //}

            //foreach (var item in entity.StringStringDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value);
            //}

            //foreach (var item in entity.StringBinaryDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value);
            //}

            //foreach (var item in entity.StringBufferedMessageDictionary)
            //{
            //    Process(item.Key);
            //    Process(item.Value.Foo);
            //    Process(item.Value.Bar);
            //}
        }

        private static void Process<T>(T entity)
        { }
    }
}
