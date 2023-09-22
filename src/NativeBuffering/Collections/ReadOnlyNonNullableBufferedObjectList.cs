using System.Collections;
using System.Runtime.CompilerServices;

namespace NativeBuffering.Collections
{
    public unsafe readonly struct ReadOnlyNonNullableBufferedObjectList<T> : IReadOnlyList<T>, IReadOnlyBufferedObject<ReadOnlyNonNullableBufferedObjectList<T>>
        where T : struct, IReadOnlyBufferedObject<T>
    {
        public NativeBuffer Buffer { get; }
        public static ReadOnlyNonNullableBufferedObjectList<T> DefaultValue { get; } = new(new NativeBuffer(new byte[4]));
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException(nameof(index));
                var position = Unsafe.Read<int>(Buffer.GetPointerByOffset(sizeof(int) * (index + 1)));
                return position == -1 ? T.DefaultValue : T.Parse(Buffer.CreateByIndex(position));
            }
        }

        public int Count => Unsafe.Read<int>(Buffer.Start);
        public ReadOnlyNonNullableBufferedObjectList(NativeBuffer buffer) => Buffer = buffer;
        public Enumerator GetEnumerator() => new(this);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public static ReadOnlyNonNullableBufferedObjectList<T> Parse(NativeBuffer buffer) => new(buffer);
        public struct Enumerator : IEnumerator<T>
        {
            private readonly NativeBuffer _buffer;
            private readonly int _count;
            private int _index = -1;
            public Enumerator(ReadOnlyNonNullableBufferedObjectList<T> collection)
            {
                _buffer = collection.Buffer;
                _count = collection.Count;
            }

            public T Current
            {
                get
                {
                    var position = Unsafe.Read<int>(_buffer.GetPointerByOffset(sizeof(int) * (_index + 1)));
                    return position == -1 ? T.DefaultValue: T.Parse(_buffer.CreateByIndex(position));
                }
            }

            object IEnumerator.Current => Current;
            public readonly void Dispose() { }
            public bool MoveNext() => ++_index < _count;
            public void Reset() => _index = -1;
        }
    }
}
