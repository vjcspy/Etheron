using System;
using System.Runtime.CompilerServices;
namespace Etheron.Utils
{
    public static class EnumUtility
    {
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static int ToIntFast<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            return unchecked((int)(object)enumValue);
        }
    }
}
