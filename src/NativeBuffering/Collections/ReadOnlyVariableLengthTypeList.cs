using System.Collections;
using System.Runtime.CompilerServices;

namespace NativeBuffering.Collections
{
    public unsafe readonly struct ReadOnlyVariableLengthTypeList<T> : IReadOnlyList<T>, IReadOnlyBufferedObject<ReadOnlyVariableLengthTypeList<T>>
        where T : IReadOnlyBufferedObject<T>
    {
        public NativeBuffer Buffer { get; }
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException(nameof(index));
                var position = Unsafe.Read<int>(Buffer.GetPointerByOffset(sizeof(int) * (index + 1)));
                return T.Parse(Buffer.CreateByIndex(position));
            }
        }

        public int Count => Unsafe.Read<int>(Buffer.Start);
        public ReadOnlyVariableLengthTypeList(NativeBuffer buffer) => Buffer = buffer;
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static ReadOnlyVariableLengthTypeList<T> Parse(NativeBuffer buffer)=> new(buffer);

        private struct Enumerator : IEnumerator<T>
        {
            private readonly NativeBuffer _buffer;
            private readonly int _count;
            private int _index = -1;
            public Enumerator(ReadOnlyVariableLengthTypeList<T> collection)
            {
                _buffer = collection.Buffer;
                _count = collection.Count;
            }

            public T Current
            {
                get
                {
                    var position = Unsafe.Read<int>(_buffer.GetPointerByOffset(sizeof(int) * (_index + 1)));
                    return T.Parse(_buffer.CreateByIndex(position));
                }
            }

            object IEnumerator.Current => Current;
            public readonly void Dispose() { }
            public bool MoveNext() => ++_index < _count;
            public void Reset() => _index = -1;
        }
    }
}
