using System.Reflection;

namespace Coralcode.Framework.ConfigManager
{
    /// <summary>
    ///     管理配置文件
    /// </summary>
    public class DllSet
    {
        internal DllSet()
        {
        }

        /// <summary>
        ///     读取或设置配置文件
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public DllTable this[string configName]
        {
            get { return new DllTable(configName); }
        }

        /// <summary>
        ///     读取当前DLL的配置文件
        /// </summary>
        public DllTable Current
        {
            get { return new DllTable(Assembly.GetCallingAssembly().GetName().Name); }
        }
    }
}