using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;

namespace NativeBuffering
{
    public  unsafe static partial class Utilities
    {      

        #region Scalar
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateUnmanagedFieldSize<T>(T _) where T : unmanaged => CalculateUnmanagedSize<T>() + sizeof(int);
        public static int CalculateUnmanagedSize<T>() where T : unmanaged => sizeof(T);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateStringFieldSize(string? value) => CalculateStringSize(value) + sizeof(int);
        public static int CalculateStringSize(string? value)
        {
            var byteCount = value is null?0: Encoding.Unicode.GetByteCount(value);

            var size = sizeof(int)
                + sizeof(nint)
                + sizeof(nint)
                + sizeof(int)
                + byteCount;
            return Math.Max(IntPtr.Size * 3 + sizeof(int), size); 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateBinaryFieldSize(IEnumerable<byte>? value) => CalculateBinarySize(value) + sizeof(int);
        public static int CalculateBinarySize(IEnumerable<byte>? value) => sizeof(int) + value?.Count() ?? 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateBufferedObjectFieldSize<T>(T value) where T : IBufferedObjectSource => value.CalculateSize() + sizeof(int);
        #endregion

        #region Collection
        public static int CalculateCollectionFieldSize<T>(IEnumerable<T>? value)=> CalculateCollectionSize(value) + sizeof(int);
        public static int CalculateCollectionSize<T>(IEnumerable<T>? value)
        {
            value = value ?? Array.Empty<T>();
            var count = value.Count();
            var lengthBytes = sizeof(int);
            var referenceBytes = sizeof(int) * count;

            if (typeof(T).IsUnmanaged())
            {
                return lengthBytes + value.Sum(_ => Unsafe.SizeOf<T>());
            }
            if (typeof(T) == typeof(string))
            {
                return lengthBytes + referenceBytes + value.Sum(it => CalculateStringSize(it as string));
            }
            if (typeof(IEnumerable<byte>).IsAssignableFrom(typeof(T)))
            {
                return lengthBytes + referenceBytes + value.Sum(it => CalculateBinarySize(it as IEnumerable<byte>));
            }
            if (typeof(IBufferedObjectSource).IsAssignableFrom(typeof(T)))
            {
                if (value.Any(it => it is null))
                {
                    throw new ArgumentException($"Specified collection contains null values.", nameof(value));
                }
                return lengthBytes + referenceBytes + value.Sum(it => ((IBufferedObjectSource)it!).CalculateSize());
            }
            throw new ArgumentException($"Unsupported collection element type {typeof(T).Name}.", nameof(value));
        }   
        #endregion

        #region Dictionary
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateDictionaryFieldSize<TKey, TValue>(IDictionary<TKey, TValue> value) => CalculateDictionarySize(value) + sizeof(int);
        public static int CalculateDictionarySize<TKey, TValue>(IDictionary<TKey, TValue> value)
        {
            if(value is null) throw new ArgumentNullException(nameof(value));
            var count = value.Count();
            var lengthBytes = sizeof(int);
            var referenceBytes = sizeof(int) * count;

            if (typeof(TKey).IsUnmanaged())
            {
                if (typeof(TValue).IsUnmanaged())
                {
                    return lengthBytes + value.Sum(kv => Unsafe.SizeOf<TKey>() + Unsafe.SizeOf<TValue>());
                }
                if (typeof(TValue) == typeof(string))
                {
                    return lengthBytes + referenceBytes + value.Sum(kv => Unsafe.SizeOf<TKey>() + CalculateStringSize(kv.Value as string));
                }
                if (typeof(IEnumerable<byte>).IsAssignableFrom(typeof(TValue)))
                {
                    return lengthBytes + referenceBytes + value.Sum(kv => Unsafe.SizeOf<TKey>() + CalculateBinarySize(kv.Value as IEnumerable<byte>));
                }
                if (typeof(IBufferedObjectSource).IsAssignableFrom(typeof(TValue)))
                {
                    if (value.Any(kv => kv.Value is null))
                    {
                        throw new ArgumentException($"Specified dictionary contains null values.", nameof(value));
                    }
                    return lengthBytes + referenceBytes + value.Sum(kv => Unsafe.SizeOf<TKey>() + ((IBufferedObjectSource)kv.Value!).CalculateSize());
                }
                throw new ArgumentException($"Unsupported dictionary value type {typeof(TValue).Name}.", nameof(value));
            }

            if (typeof(TKey) == typeof(string))
            {
                if (typeof(TValue).IsUnmanaged())
                {
                    return lengthBytes + referenceBytes + value.Sum(kv => CalculateStringSize(kv.Key as string) + Unsafe.SizeOf<TValue>());
                }
                if (typeof(TValue) == typeof(string))
                {
                    return lengthBytes + referenceBytes + value.Sum(kv => CalculateStringSize(kv.Key as string) + CalculateStringSize(kv.Value as string));
                }
                if (typeof(IEnumerable<byte>).IsAssignableFrom(typeof(TValue)))
                {
                    return lengthBytes + referenceBytes + value.Sum(kv => CalculateStringSize(kv.Key as string) + CalculateBinarySize(kv.Value as IEnumerable<byte>));
                }
                if (typeof(IBufferedObjectSource).IsAssignableFrom(typeof(TValue)))
                {
                    if (value.Any(kv => kv.Value is  null))
                    {
                        throw new ArgumentException($"Specified dictionary contains null values.", nameof(value));
                    }
                    return lengthBytes + referenceBytes + value.Sum(kv => CalculateStringSize(kv.Key as string) + ((IBufferedObjectSource)kv.Value!).CalculateSize());
                }
                throw new ArgumentException($"Unsupported type {typeof(TValue).Name}.", nameof(value));
            }

          throw new ArgumentException($"Unsupported dictionary key type {typeof(TKey).Name}.", nameof(value));
        }
        #endregion
    }
}
