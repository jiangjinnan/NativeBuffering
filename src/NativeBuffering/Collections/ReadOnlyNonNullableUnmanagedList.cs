using System.Collections;
using System.Runtime.CompilerServices;

namespace NativeBuffering.Collections
{
    public readonly unsafe struct ReadOnlyNonNullableUnmanagedList<T> : IReadOnlyList<T>, IReadOnlyBufferedObject<ReadOnlyNonNullableUnmanagedList<T>>
        where T : unmanaged
    {
        private static readonly int _elementSize = Unsafe.SizeOf<T>() % AlignmentCalculator.AlignmentOf<T>() == 0 ? Unsafe.SizeOf<T>() : Unsafe.SizeOf<T>() + AlignmentCalculator.AlignmentOf<T>() - Unsafe.SizeOf<T>() % AlignmentCalculator.AlignmentOf<T>();
        public NativeBuffer Buffer { get; }
        public static ReadOnlyNonNullableUnmanagedList<T> DefaultValue { get; } = new(new NativeBuffer(new byte[4]));
        public readonly int Count => Unsafe.Read<int>(Buffer.Start);
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException(nameof(index));
                return Unsafe.Read<T>(Buffer.GetPointerByOffset(IntPtr.Size + index * _elementSize));
            }
        }

        public readonly ref T AsRef(int index)
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException(nameof(index));
            return ref Unsafe.AsRef<T>(Buffer.GetPointerByOffset(IntPtr.Size + index * _elementSize));
        }

        public ReadOnlyNonNullableUnmanagedList(NativeBuffer buffer) => Buffer = buffer;
        public Enumerator GetEnumerator()=> new Enumerator(this);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public static ReadOnlyNonNullableUnmanagedList<T> Parse(NativeBuffer buffer) => new(buffer);

        public struct Enumerator : IEnumerator<T>
        {
            private readonly void* _start;
            private readonly int _count;
            private int _index = -1;

            public Enumerator(ReadOnlyNonNullableUnmanagedList<T> list)
            {
                _start = list.Buffer.Start;
                _count = list.Count;
            }

            public readonly T Current => Unsafe.Read<T>(Unsafe.Add<byte>(_start, IntPtr.Size + _elementSize * _index));
            readonly object IEnumerator.Current => Current;
            public readonly void Dispose() { }
            public bool MoveNext() => ++_index < _count;
            public void Reset() => _index = -1;
        }
    }
}
