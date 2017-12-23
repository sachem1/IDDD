using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Routing;
using Coralcode.Framework.Aspect;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.ConfigManager;
using Coralcode.Framework.Reflection;
using iTextSharp.text.io;
using Coralcode.Framework.Data;

namespace Coralcode.Framework.Modules
{
    /// <summary>
    /// 模块，用于dll数据库分库
    /// </summary>
    public abstract class CoralModule
    {
        private List<Type> _types;

        private Assembly _assembly;
        /// <summary>
        /// 获取当前Module相关的程序集
        /// </summary>
        public virtual Assembly Assembly
        {
            get
            {
                if (_assembly == null)
                    _assembly = GetType().Assembly;
                return _assembly;
            }

        }

        /// <summary>
        /// 获取当前Module相关的类型
        /// </summary>
        /// <returns></returns>
        public virtual List<Type> Types
        {
            get { return _types ?? (_types = Assembly.GetTypes().Distinct().ToList()); }
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        public virtual string Name
        {
            get
            {
                return GetModuleName(GetType());
            }
        }
        
        /// <summary>
        /// 准备 主要做ioc加载
        /// </summary>
        public virtual void Prepare() { }

        /// <summary>
        /// 装载 
        /// </summary>
        public virtual void Install() { }


        /// <summary>
        /// 装载完毕
        /// </summary>
        public virtual void Installed() { }

        /// <summary>
        /// 卸载
        /// </summary>
        public virtual void UnInstall() { }

        internal static Func<Type, bool> IsCoralModule
        {
            get { return item => item.IsSubclassOf(typeof(CoralModule)) && !item.IsAbstract; }
        }

        protected string GetModuleName(Type moduleType)
        {
            return moduleType.Name.Replace("Module", "");
        }
    }
}
