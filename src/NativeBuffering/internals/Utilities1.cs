using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace NativeBuffering
{
    public  unsafe static partial class Utilities
    {
        public static bool IsUnmanaged<T>() => IsUnmanaged(typeof(T));
        public static bool IsUnmanaged(this Type type)
        {
            if(!type.IsValueType) return false;
            if (type.IsPrimitive || type.IsPointer || type.IsEnum) return true;
            if (type.IsGenericType || !type.IsValueType) return true;
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).All(x => x.FieldType.IsUnmanaged());
        }      
    }
}
