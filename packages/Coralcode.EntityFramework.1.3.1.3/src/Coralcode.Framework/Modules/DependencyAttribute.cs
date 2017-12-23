using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;

namespace Coralcode.Framework.Modules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependencyAttribute : Attribute
    {

        /// <summary>
        /// 依赖的module类型
        /// </summary>
        /// <param name="moduleTypes"></param>
        public DependencyAttribute(params Type[] moduleTypes)
        {

            ModuleTypes = moduleTypes;
        }
        /// <summary>
        ///  依赖的module类型
        /// </summary>
        public Type[] ModuleTypes { get; private set; }

        public static IEnumerable<Type> GetDependencies(Type type)
        {
            var attrs = type.GetCustomAttributes<DependencyAttribute>();
            return attrs.SelectMany(item => item.ModuleTypes);
        }
    }
}
