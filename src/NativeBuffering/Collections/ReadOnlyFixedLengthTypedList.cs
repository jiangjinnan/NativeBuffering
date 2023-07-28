using System.Collections;
using System.Runtime.CompilerServices;

namespace NativeBuffering.Collections
{
    public readonly unsafe struct ReadOnlyFixedLengthTypedList<T> : IReadOnlyList<T>, IReadOnlyBufferedObject<ReadOnlyFixedLengthTypedList<T>>
        where T: unmanaged
    {        
        public NativeBuffer Buffer { get; }
        public readonly int Count => Unsafe.Read<int>(Buffer.Start);
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException(nameof(index));
                return Unsafe.Read<T>(Buffer.GetPointerByOffset(sizeof(int) +  index  * Unsafe.SizeOf<T>()));
            }
        }

        public readonly ref T AsRef(int index)
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException(nameof(index));
            return ref Unsafe.AsRef<T>(Buffer.GetPointerByOffset(sizeof(int) +  index  * Unsafe.SizeOf<T>()));
        }

        public ReadOnlyFixedLengthTypedList(NativeBuffer buffer) => Buffer = buffer;
        public IEnumerator<T> GetEnumerator()=> new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator()=> GetEnumerator();

        public static ReadOnlyFixedLengthTypedList<T> Parse(NativeBuffer buffer)=> new(buffer);

        private struct Enumerator : IEnumerator<T>
        {
            private readonly void* _start;
            private readonly int _count;
            private int _index = -1;

            public Enumerator(ReadOnlyFixedLengthTypedList<T> list)
            {
                _start = list.Buffer.Start;
                _count = list.Count;
            }

            public readonly T Current => Unsafe.Read<T>(Unsafe.Add<byte>(_start, sizeof(int) + Unsafe.SizeOf<T>() * _index));
            readonly object IEnumerator.Current => Current;
            public readonly void Dispose() { }
            public bool MoveNext() => ++_index < _count;
            public void Reset() => _index = -1;
        }
    }
}
