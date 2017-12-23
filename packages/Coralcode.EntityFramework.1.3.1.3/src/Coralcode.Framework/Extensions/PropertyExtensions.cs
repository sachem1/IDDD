using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Coralcode.Framework.Extensions
{
    public static class PropertyExtensions
    {
        public static string GetDescription(this MemberInfo property)
        {
            if (property == null)
                return null;
            var desc = property.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().FirstOrDefault();
            return desc == null ? "" : desc.Description;
        }


    }
}
