using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace Coralcode.Framework.ConfigManager
{
    /// <summary>
    ///     管理配置文件条目下的配置项
    /// </summary>
    public class DllItem : IEnumerable<XElement>
    {
        private readonly Configuration _config;
        private readonly string _configItemName;
        private XElement _root;

        internal DllItem(Configuration config, string configItemName)
        {
            this._config = config;
            this._configItemName = configItemName;
            Refresh();
        }

        /// <summary>
        ///     读取或设置配置文件条目下的配置项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                var xNode = this.FirstOrDefault(d => d.Name == "add" && d.Attribute("key").Value == key);
                if (xNode == null)
                    return null;
                return xNode.Attribute("value").Value;
            }
            set
            {
                var xNode = this.FirstOrDefault(d => d.Name == "add" && d.Attribute("key").Value == key);
                if (xNode == null)
                    _root.Add(new XElement("add", new XAttribute("key", key), new XAttribute("value", value)));
                else
                    xNode.Attribute("value").Value = value;
            }
        }

        #region IEnumerable<XElement> 成员

        public IEnumerator<XElement> GetEnumerator()
        {
            foreach (var xNode in _root.Elements())
                yield return xNode;
        }

        #endregion

        #region IEnumerable 成员

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        ///     保存配置信息
        /// </summary>
        public void Save()
        {
            //得到配置节点
            var section = _config.GetSection(_configItemName);
            var xdoc = XDocument.Parse(section.SectionInformation.GetRawXml());
            //写入配置节点
            xdoc.Elements().First().ReplaceAll(this);

            section.SectionInformation.SetRawXml(xdoc.ToString());
            _config.Save();
        }

        /// <summary>
        ///     重新读取配置信息
        /// </summary>
        public void Refresh()
        {
            var section = _config.GetSection(_configItemName);
            var xdoc = XDocument.Parse(section.SectionInformation.GetRawXml());
            _root = xdoc.Elements().First();
        }

        /// <summary>
        ///     返回全部配置项的键值对
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ToDictionary()
        {
            return _root.Elements("add").ToDictionary(d => d.Attribute("key").Value, d => d.Attribute("value").Value);
        }

        public bool IsExists(string configItemName)
        {
            return _root.Elements("add").FirstOrDefault(d => d.Attribute("key").Value == configItemName) != null;
        }
    }
}