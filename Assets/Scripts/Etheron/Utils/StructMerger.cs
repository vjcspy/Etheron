using System;
using System.Reflection;
namespace Etheron.Utils
{
    public static class StructMerger
    {
        public static T Merge<T>(T primary, T secondary) where T : struct
        {
            Type type = typeof(T);
            T result = primary;

            foreach (FieldInfo field in type.GetFields(bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object primaryValue = field.GetValue(obj: primary);
                object defaultValue = GetDefaultValue(type: field.FieldType);

                // Nếu giá trị trong primary là default, thì lấy từ secondary
                if (Equals(objA: primaryValue, objB: defaultValue))
                {
                    object secondaryValue = field.GetValue(obj: secondary);
                    field.SetValueDirect(obj: __makeref(result), value: secondaryValue);
                }
            }

            return result;
        }

        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type: type) : null;
        }
    }
}
