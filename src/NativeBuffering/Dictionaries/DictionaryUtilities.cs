using System.Runtime.CompilerServices;

namespace NativeBuffering
{
    internal unsafe static class DictionaryUtilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static int GetEntrySlotCount<TKey, TValue>(IDictionary<TKey, TValue> dictionary) => Utilities.GetDictionaryEntryCount(dictionary.Count);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static int GetCount(NativeBuffer buffer) => Unsafe.Read<int>(buffer.Start);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static int GetEntrySlotCount(NativeBuffer buffer) => Unsafe.Read<int>(buffer.GetPointerByOffset(sizeof(int)));
    }
}
