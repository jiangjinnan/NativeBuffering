using NativeBuffering.Dictionaries;

namespace NativeBuffering.Test
{
    public class StringBufferedObjectDictionaryFixture
    {
        [Fact]
        public void GetValues()
        { 
            var source = new Source(new Dictionary<string, Foobar> { { "1", new Foobar(1,1) } , { "2", new Foobar(2,2) }, { "3", new Foobar(3,3) }
            });
            var buffer = new byte[source.CalculateSize()];
            var context = new BufferedObjectWriteContext(buffer);
            source.Write(context);
            var message = Message.Parse(new NativeBuffer(buffer, 0));
            Assert.Equal(3, message.Value.Count);
            Assert.Equal(1, message.Value["1"].Foo);
            Assert.Equal(2, message.Value["2"].Foo);
            Assert.Equal(3, message.Value["3"].Foo);
            Assert.Equal(1, message.Value["1"].Bar);
            Assert.Equal(2, message.Value["2"].Bar);
            Assert.Equal(3, message.Value["3"].Bar);
        }

        public class Source : IBufferedObjectSource
        {
            public Source(Dictionary<string, Foobar> value)
            {
                Value = value;
            }

            public Dictionary<string, Foobar> Value { get; }
            public int CalculateSize() => Utilities.CalculateDictionaryFieldSize(Value);

            public void Write(BufferedObjectWriteContext context)
            {
                using var scope = new BufferedObjectWriteContextScope(context);
                scope.WriteStringBufferedObjectDictionaryField(Value);
            }
        }

        public class Message: IReadOnlyBufferedObject<Message>
        {
            public Message(NativeBuffer buffer) => Buffer = buffer;
            public NativeBuffer Buffer { get; }
            public ReadOnlyStringBufferedObjectDictionary<FoobarMessage> Value => Buffer.ReadStringBufferedObjectDictionaryField<FoobarMessage>(0);
            public static Message Parse(NativeBuffer buffer) => new Message(buffer);
        }

        public class Foobar : IBufferedObjectSource
        {
            public Foobar(int foo, long bar)
            {
                Foo = foo;
                Bar = bar;
            }

            public int Foo { get; }
            public long Bar { get; }
            public int CalculateSize() => Utilities.CalculateUnmanagedFieldSize(Foo) + Utilities.CalculateUnmanagedFieldSize(Bar);

            public void Write(BufferedObjectWriteContext context)
            {
                using var scope = new BufferedObjectWriteContextScope(context);
                scope.WriteUnmanagedField(Foo);
                scope.WriteUnmanagedField(Bar);
            }
        }

        public class FoobarMessage : IReadOnlyBufferedObject<FoobarMessage>
        {
            public NativeBuffer Buffer { get; }
            public FoobarMessage(NativeBuffer buffer) => Buffer = buffer;
            public static FoobarMessage Parse(NativeBuffer buffer) => new FoobarMessage(buffer);

            public int Foo => Buffer.ReadUnmanagedField<int>(0);
            public long Bar => Buffer.ReadUnmanagedField<long>(1);
        }
    }
}
