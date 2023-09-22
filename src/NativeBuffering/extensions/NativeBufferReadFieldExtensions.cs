using NativeBuffering.Collections;
using NativeBuffering.Dictionaries;
using System.Runtime.CompilerServices;

namespace NativeBuffering
{
    public unsafe static class NativeBufferReadFieldExtensions
    {
        #region Unmanaged
        public static T ReadUnmanagedField<T>(this NativeBuffer buffer, int index) where T : unmanaged
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return Unsafe.Read<T>(buffer.GetPointerByIndex(position));
        }

        public static T? ReadNullableUnmanagedField<T>(this NativeBuffer buffer, int index) where T : unmanaged
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return position == -1 ? null : Unsafe.Read<T>(buffer.GetPointerByIndex(position));
        }

        public static ref T ReadUnmanagedFieldAsRef<T>(this NativeBuffer buffer, int index) where T : unmanaged
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return ref Unsafe.AsRef<T>(buffer.GetPointerByIndex(position));
        }
        #endregion

        #region BufferedObject
        public static T ReadNonNullableBufferedObjectField<T>(this NativeBuffer buffer, int index) where T : struct, IReadOnlyBufferedObject<T>
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return position == -1 ? T.DefaultValue: T.Parse(buffer.CreateByIndex(position));
        }

        public static T? ReadNullableBufferedObjectField<T>(this NativeBuffer buffer, int index) where T : struct, IReadOnlyBufferedObject<T>
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return position == -1 ? null : T.Parse(buffer.CreateByIndex(position));
        }
        #endregion

        public static ReadOnlyNonNullableUnmanagedList<T> ReadNonNullableUnmanagedCollectionField<T>(this NativeBuffer buffer, int index) where T : unmanaged
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return new ReadOnlyNonNullableUnmanagedList<T>(buffer.CreateByIndex(position));
        }

        public static ReadOnlyNullableUnmanagedList<T> ReadNullableUnmanagedCollectionField<T>(this NativeBuffer buffer, int index) where T : unmanaged
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return new ReadOnlyNullableUnmanagedList<T>(buffer.CreateByIndex(position));
        }

        public static ReadOnlyNonNullableBufferedObjectList<T> ReadNonNullableBufferedObjectCollectionField<T>(this NativeBuffer buffer, int index) where T : struct, IReadOnlyBufferedObject<T>
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return position == -1 ? ReadOnlyNonNullableBufferedObjectList<T>.DefaultValue : new ReadOnlyNonNullableBufferedObjectList<T>(buffer.CreateByIndex(position));
        }

        public static ReadOnlyNullableBufferedObjectList<T> ReadNullableBufferedObjectCollectionField<T>(this NativeBuffer buffer, int index) where T : struct, IReadOnlyBufferedObject<T>
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return position == -1 ? ReadOnlyNullableBufferedObjectList<T>.DefaultValue : new ReadOnlyNullableBufferedObjectList<T>(buffer.CreateByIndex(position));
        }

        public static ReadOnlyUnmanagedUnmanagedDictionary<TKey, TValue> ReadUnmanagedUnmanagedDictionaryField<TKey, TValue>(this NativeBuffer buffer, int index)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return new ReadOnlyUnmanagedUnmanagedDictionary<TKey, TValue>(buffer.CreateByIndex(position));
        }

        public static ReadOnlyUnmanagedBufferedObjectDictionary<TKey, TValue> ReadUnmanagedBufferedObjectDictionaryField<TKey, TValue>(this NativeBuffer buffer, int index)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : IReadOnlyBufferedObject<TValue>
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return new ReadOnlyUnmanagedBufferedObjectDictionary<TKey, TValue>(buffer.CreateByIndex(position));
        }

        public static ReadOnlyStringUnmanagedDictionary<TValue> ReadStringUnmanagedDictionaryField<TValue>(this NativeBuffer buffer, int index)
            where TValue : unmanaged
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return new ReadOnlyStringUnmanagedDictionary<TValue>(buffer.CreateByIndex(position));
        }

        public static ReadOnlyStringBufferedObjectDictionary<TValue> ReadStringBufferedObjectDictionaryField<TValue>(this NativeBuffer buffer, int index)
            where TValue : IReadOnlyBufferedObject<TValue>
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return new ReadOnlyStringBufferedObjectDictionary<TValue>(buffer.CreateByIndex(position));
        }
    }
}