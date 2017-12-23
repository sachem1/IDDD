using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.Framework.Extensions
{
    public static class TypeExtensions
    {

        public static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive ||
                   type == typeof(string) ||
                   type == typeof(DateTime) ||
                   type == typeof(Decimal) ||
                   type == typeof(Guid) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan)||
                   type.IsEnum;
        }

        public static bool IsSimpleUnderlyingType(this Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return IsSimpleType(underlyingType ?? type);
        }
        public static bool IsStream(this Type type)
        {
            return type.IsSubclassOf(typeof(Stream))||type==typeof(Stream);
        }
    }
}
