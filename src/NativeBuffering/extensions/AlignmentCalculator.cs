using System.Collections.Concurrent;
using System.Reflection;

namespace NativeBuffering
{
    internal static  class AlignmentCalculator
    {
        private static readonly ConcurrentDictionary<Type, int> _alignmentCache = new();
        public static int AlignmentOf<T>() where T : unmanaged => CalculateAlignment(typeof(T));
        private static int CalculateAlignment(Type type)=> _alignmentCache.GetOrAdd(type, CalculateAlignmentCore);
        private static int CalculateAlignmentCore(Type type)
        {
            if (type.IsPrimitive)
            {
                return type == typeof(bool) ? 1 : type == typeof(double) || type == typeof(long) || type == typeof(ulong) ? 8 : 4;
            }
            else
            {
                var maxAlignment = 0;
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    var fieldAlignment = CalculateAlignment(field.FieldType);
                    if (fieldAlignment > maxAlignment)
                    {
                        maxAlignment = fieldAlignment;
                    }
                }
                return maxAlignment;
            }
        }
    }
}
