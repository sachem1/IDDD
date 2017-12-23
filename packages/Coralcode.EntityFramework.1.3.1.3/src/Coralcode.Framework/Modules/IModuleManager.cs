using System;
using System.Collections.Generic;

namespace Coralcode.Framework.Modules
{
    /// <summary>
    /// 模块管理
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// 根据模块中的某个类型获取模块
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        CoralModule GetModule(Type type);

        /// <summary>
        /// 根据模块名字获取
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        CoralModule GetModule(string moduleName);

        /// <summary>
        /// 获取模块
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        CoralModule GetModule(Func<CoralModule, bool> predicate);

        /// <summary>
        /// 获取多个模块
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<CoralModule> GetModules(Func<CoralModule, bool> predicate);

        /// <summary>
        /// 加载所有的模块
        /// </summary>
        void InstallModules();

        /// <summary>
        /// 卸载所有的module
        /// </summary>
        void UninstallModules();
    }
}
