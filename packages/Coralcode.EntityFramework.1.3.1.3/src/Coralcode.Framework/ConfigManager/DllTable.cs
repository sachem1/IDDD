using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Coralcode.Framework.ConfigManager
{
    /// <summary>
    ///     管理配置文件条目
    /// </summary>
    public class DllTable : IEnumerable<DllItem>
    {
        private readonly Configuration config;

        internal DllTable(string configName)
        {
            //得到配置文件路径
            var configFile = new ExeConfigurationFileMap();
            configFile.ExeConfigFilename = AppConfig.GetAbsolutePath(configName);
            //得到配置文件
            config = ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
        }

        /// <summary>
        ///     读取或设置配置文件条目
        /// </summary>
        /// <param name="configItemName"></param>
        /// <returns></returns>
        public DllItem this[string configItemName]
        {
            get
            {
                //得到配置节点
                return new DllItem(config, configItemName);
            }
            set
            {
                //得到配置节点
                value.Save();
            }
        }

        public IEnumerator<DllItem> GetEnumerator()
        {
            return (from ConfigurationSection section in config.Sections
                    where !string.IsNullOrEmpty(section.SectionInformation.GetRawXml())
                    select this[section.SectionInformation.SectionName]).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsExists(string configItemName)
        {
            var section = config.Sections[configItemName];
            return section != null;
        }
    }
}