using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Reflection;
using System.ComponentModel.DataAnnotations;

namespace Coralcode.Framework.Data
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
        public static bool IsDefined(Type type)
        {
            return Attribute.IsDefined(type, typeof(IgnoreAttribute), false);
        }

        public static bool IsDefined(MemberInfo member)
        {
            return Attribute.IsDefined(member, typeof(IgnoreAttribute), false);
        }

        public static string[] GetIgnoreMembers(Type type)
        {
            return type.GetProperties().Where(IsDefined).Select(item => item.Name).ToArray();
        }

        public static bool IsRequired(MemberInfo member)
        {
            return Attribute.IsDefined(member, typeof(RequiredAttribute), false);
        }

        public static string[] GetRequiredMembers(Type type)
        {
            return type.GetProperties().Where(IsRequired).Select(item => item.Name).ToArray();
        }

    }
}
