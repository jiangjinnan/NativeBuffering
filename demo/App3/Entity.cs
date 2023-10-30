using NativeBuffering;

namespace App3
{
    [BufferedMessageSource]
    public partial class Entity
    {
        // Primitive 
        public byte ByteValue { get; set; }
        public sbyte SByteValue { get; set; }
        public short ShortValue { get; set; }
        public ushort UShortValue { get; set; }
        public int IntValue { get; set; }
        public uint UIntValue { get; set; }
        public long LongValue { get; set; }
        public ulong ULongValue { get; set; }
        public float FloatValue { get; set; }
        public double DoubleValue { get; set; }
        public decimal DecimalValue { get; set; }
        public char CharValue { get; set; }
        public bool BoolValue { get; set; }
        public DateTimeKind EnumValue { get; set; }

        // Unmanaged struct
        public TimeSpan TimeSpanValue { get; set; }

        // String
        public string StringValue { get; set; } = default!;

        // Binary
        public byte[] ByteArrayValue { get; set; } = default!;
        //public Memory<byte> MemoryOfByteValue { get; set; }

        // Collection
        public IEnumerable<Foobar> CollectionValue { get; set; } = default!;

        //Dictionary
        public IDictionary<int, Foobar> DictionaryValue { get; set; } = default!;
    }

    [BufferedMessageSource]
    public partial class Foobar
    { 
        public int Foo { get; set; }
        public long Bar { get; set; }
    }
}
