using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.Framework.Reflection
{
    /// <summary>
    /// 反射相关扩展
    /// </summary>
    public static class ReflectionExtensions
    {
        #region Description
        public static string GetDescription(this MemberInfo type)
        {
            var desc = type.GetCustomAttribute<DescriptionAttribute>();
            return desc == null ? null : desc.Description;
        }
        #endregion
    }
}
