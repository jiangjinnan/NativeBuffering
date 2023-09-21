using NativeBuffering.Collections;
using NativeBuffering.Dictionaries;
using System.Runtime.CompilerServices;

namespace NativeBuffering
{
    public unsafe static class NativeBufferReadFieldExtensions
    {
        public static T ReadUnmanagedField<T>(this NativeBuffer buffer, int index) where T : unmanaged
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return Unsafe.Read<T>(buffer.GetPointerByIndex(position));
        }

        public static ref T ReadUnmanagedFieldAsRef<T>(this NativeBuffer buffer, int index) where T : unmanaged
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return ref Unsafe.AsRef<T>(buffer.GetPointerByIndex(position));
        }
        public static T ReadBufferedObjectField<T>(this NativeBuffer buffer, int index) where T : IReadOnlyBufferedObject<T>
        { 
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return T.Parse(buffer.CreateByIndex(position));
        }

        public static ReadOnlyFixedLengthTypedList<T> ReadUnmanagedCollectionField<T>(this NativeBuffer buffer, int index) where T : unmanaged
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return new ReadOnlyFixedLengthTypedList<T>(buffer.CreateByIndex(position));
        }

        public static ReadOnlyVariableLengthTypeList<T> ReadBufferedObjectCollectionField<T>(this NativeBuffer buffer, int index) where T : IReadOnlyBufferedObject<T>
        {
            var position = Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int) * index));
            return new ReadOnlyVariableLengthTypeList<T>(buffer.CreateByIndex(position));
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