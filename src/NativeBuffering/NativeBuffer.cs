using System.Runtime.CompilerServices;

namespace NativeBuffering
{
    public unsafe readonly struct NativeBuffer
    {
        public byte[] Bytes { get; }
        private readonly int _index;

        public void* Start
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Unsafe.AsPointer(ref Bytes[_index]);
        }

        public NativeBuffer(byte[] bytes, int index = 0)
        {
            Bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
            _index = index;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* GetPointerByIndex(int index) => Unsafe.AsPointer(ref Bytes[index]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* GetPointerByOffset(int offset) => Unsafe.Add<byte>(Start, offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeBuffer CreateByIndex(int index) => new(Bytes, index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeBuffer CreateByOffset(int offset) => new(Bytes, _index + offset);

        public byte[] ToByteArray()
        { 
            var end = new nint( Unsafe.AsPointer(ref Bytes[Bytes.Length -1]));
            var count = end - new nint(Start) + 1;
            var result = new byte[count];
            Unsafe.CopyBlockUnaligned(ref result[0], ref Bytes[Bytes.Length - count], (uint)count);
            return result;
        }
    }
}
