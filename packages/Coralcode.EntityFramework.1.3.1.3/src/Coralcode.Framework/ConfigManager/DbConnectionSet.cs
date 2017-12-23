using System.Configuration;

namespace Coralcode.Framework.ConfigManager
{
    /// <summary>
    ///     管理连接字符串
    /// </summary>
    public class DbConnectionSet
    {
        internal DbConnectionSet()
        {
        }

        /// <summary>
        ///     读取连接字符串
        /// </summary>
        /// <param name="connName"></param>
        /// <returns></returns>
        public string this[string connName]
        {
            get { return ConfigurationManager.ConnectionStrings[connName].ConnectionString; }
        }
    }
}