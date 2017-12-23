using System.Configuration;

namespace Coralcode.Framework.ConfigManager
{
    /// <summary>
    ///     管理AppSettings节点
    /// </summary>
    public class AppSet
    {
        internal AppSet()
        {
        }

        /// <summary>
        ///     获取AppSettings下的节点值
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string this[string keyName]
        {
            get { return ConfigurationManager.AppSettings.Get(keyName); }
        }
    }
}