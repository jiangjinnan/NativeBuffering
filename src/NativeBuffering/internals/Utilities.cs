using System.Runtime.CompilerServices;

namespace NativeBuffering
{
    public unsafe static partial class Utilities
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetDictionaryEntryIndex<T>(this T value, int entrySlotCount)=>(int)((uint)value!.GetHashCode() % entrySlotCount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetDictionaryEntryCount(int count)
        {
            if (count < 0)
            {
                throw new ArgumentException(message: "Value must be greater than or equal to 0.", paramName: nameof(count));
            }
            int[] array = _primes;
            foreach (int num in array)
            {
                if (num >= count)
                {
                    return num;
                }
            }
            for (int j = count | 1; j < int.MaxValue; j += 2)
            {
                if (IsPrime(j) && (j - 1) % 101 != 0)
                {
                    return j;
                }
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPrime(int candidate)
        {
            if (((uint)candidate & (true ? 1u : 0u)) != 0)
            {
                int num = (int)Math.Sqrt(candidate);
                for (int i = 3; i <= num; i += 2)
                {
                    if (candidate % i == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            return candidate == 2;
        }

        private static readonly int[] _primes = new int[72]
        {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71,
            89, 107, 131, 163, 197, 239, 293, 353, 431, 521,
            631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371,
            4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023,
            25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363,
            156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403,
            968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559,
            5999471, 7199369
        };
    }
}
