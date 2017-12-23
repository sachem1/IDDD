using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.Framework.GenericsFactory
{
    public interface IStrategy
    {
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class StrategyAttribute : Attribute
    {
        public StrategyAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; private set; }

        public StrategyType StrategyType { get;set; }
    }

    public enum StrategyType
    {
        /// <summary>
        /// 传入
        /// </summary>
        In,
        /// <summary>
        /// 传出
        /// </summary>
        Out,
    }

    public static class ReflectionExtensions
    {
        #region Description

        /// <summary>
        /// 获取类型的策略标记
        /// 如果为空则返回当前类型的名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetStrategy(this MemberInfo type)
        {
            var desc = type.GetCustomAttribute<StrategyAttribute>();
            return desc == null ? type.Name : desc.Description;
        }

        public static List<string> GetStrategys(this MemberInfo type)
        {
            var descs = type.GetCustomAttributes<StrategyAttribute>().ToList();

            return descs.Count == 0 ? null : descs.Select(item => item.Description).ToList();
        }

        #endregion
    }
}
