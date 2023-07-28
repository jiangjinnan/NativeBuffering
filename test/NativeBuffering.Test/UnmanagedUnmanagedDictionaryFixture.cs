﻿using NativeBuffering.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeBuffering.Test
{
    public class UnmanagedUnmanagedDictionaryFixture
    {
        [Fact]
        public void GetValues()
        { 
            var source = new Source(new Dictionary<long, long> { { 1, 1 }, { 2, 2 }, { 3, 3 } });
            var buffer = new byte[source.CalculateSize()];
            var context = new BufferedObjectWriteContext(buffer);
            source.Write(context);
            var message = Message.Parse(new NativeBuffer(buffer, 0));
            Assert.Equal(3, message.Value.Count);
            Assert.Equal(1, message.Value[1]);
            Assert.Equal(2, message.Value[2]);
            Assert.Equal(3, message.Value[3]);
        }

        public class Source : IBufferedObjectSource
        {
            public Source(Dictionary<long, long> value)
            {
                Value = value;
            }

            public Dictionary<long, long> Value { get; }
            public int CalculateSize() => Utilities.CalculateDictionaryFieldSize(Value);

            public void Write(BufferedObjectWriteContext context)
            {
                using var scope = new BufferedObjectWriteContextScope(context);
                scope.WriteUnmanagedUnmanagedDictionaryField(Value);
            }
        }

        public class Message: IReadOnlyBufferedObject<Message>
        {
            public Message(NativeBuffer buffer) => Buffer = buffer;
            public NativeBuffer Buffer { get; }
            public ReadOnlyUnmanagedUnmanagedDictionary<long, long> Value => Buffer.ReadUnmanagedUnmanagedDictionaryField<long, long>(0);
            public static Message Parse(NativeBuffer buffer) => new Message(buffer);
        }
    }
}
