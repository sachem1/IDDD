using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Coralcode.Framework.Common;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Reflection;

namespace Coralcode.Framework.Serializers.Dynamic
{
    public interface IConfig
    {
        string PathOrSourceString { get; set; }

        dynamic Data { get; set; }
    }


    public static class ConfigManager
    {
        public static IConfig LoadFromFile(this IConfig config)
        {
            if (config == null || string.IsNullOrEmpty(config.PathOrSourceString))
                throw new ArgumentNullException("config");
            if (!File.Exists(config.PathOrSourceString))
            {
                return config;
            }
            var doc = new XmlDocument();
            doc.Load(config.PathOrSourceString);
            var element = doc["Data"];
            config.Data = GetValue(element);
            return config;
        }

        public static IConfig SaveToFile(this IConfig config)
        {
            if (config == null || string.IsNullOrEmpty(config.PathOrSourceString) || config.Data == null)
                throw new ArgumentNullException("config");
            var dir = Path.GetDirectoryName(config.PathOrSourceString);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var doc = new XmlDocument();
            doc.AppendChild(GetXml("Data", config.Data, doc));
            doc.Save(config.PathOrSourceString);
            return config;
        }

        public static IConfig LoadFromString(this IConfig config)
        {
            if (config == null || string.IsNullOrEmpty(config.PathOrSourceString))
                throw new ArgumentNullException("config");
            var doc = new XmlDocument();
            doc.LoadXml(config.PathOrSourceString);
            var element = doc["Data"];
            config.Data = GetValue(element);
            return config;
        }

        public static IConfig SaveToString(this IConfig config)
        {
            if (config == null || config.Data == null)
                throw new ArgumentNullException("config");
            var doc = new XmlDocument();
            doc.AppendChild(GetXml("Data", config.Data, doc));
            config.PathOrSourceString = doc.OuterXml;
            return config;
        }

        #region 解析XmlElement

        public static dynamic GetValue(XmlElement element)
        {
            if (element == null)
                return null;

            Classify clasify;
            Enum.TryParse(element.GetAttribute("Classify"), out clasify);
            switch (clasify)
            {
                case Classify.Sample:
                    return GetSampleValue(element.GetAttribute("Assembly"), element.GetAttribute("Type"), element.InnerText);
                case Classify.Array:
                    return GetArrayValue(element.GetAttribute("ElementAssembly"), element.GetAttribute("ElementType"), element.GetChidlren());
                case Classify.List:
                    return GetListValue(element.GetAttribute("GenericAssembly"), element.GetAttribute("GenericType"), element.GetChidlren());
                case Classify.Dictionary:
                    return GetDictionaryValue(element.GetAttribute("KeyGenericAssembly"),
                        element.GetAttribute("KeyGenericType"),
                        element.GetAttribute("ValueGenericAssembly"),
                        element.GetAttribute("ValueGenericType"),
                        element.GetChidlren());
                case Classify.Dynamic:
                    return GetDynamicValue(element.GetChidlren());
                case Classify.Custom:
                    return GetCustomValue(element.GetAttribute("Assembly"), element.GetAttribute("Type"), element.GetChidlren());
            }

            return null;
        }
        public static object GetSampleValue(string assembly, string typeFullName, string value)
        {
            var type = Assembly.Load(assembly).GetType(typeFullName);
            if (type == null)
                return null;
            return CoralConvert.Convert(value, type);
        }
        public static object GetListValue(string genericAssembly, string genericTypeName, List<XmlElement> elements)
        {
            var genericType = Assembly.Load(genericAssembly).GetType(genericTypeName);
            var type = typeof(List<>).MakeGenericType(genericType);
            dynamic list = Activator.CreateInstance(type, true);

            foreach (var element in elements)
            {
                list.Add(GetValue(element));
            }
            return list;
        }
        public static object GetArrayValue(string elementAssembly, string elementTypeName, List<XmlElement> elements)
        {
            var elementType = Assembly.Load(elementAssembly).GetType(elementTypeName);
            dynamic list = Array.CreateInstance(elementType, elements.Count);
            for (int i = 0; i < elements.Count; i++)
            {
                list[i] = GetValue(elements[i]);
            }
            return list;
        }
        public static object GetDictionaryValue(string keyAssembly, string keyTypeName, string valueAssembly, string valueTypeName, List<XmlElement> elements)
        {
            var keyType = Assembly.Load(keyAssembly).GetType(keyTypeName);
            var valueType = Assembly.Load(valueAssembly).GetType(valueTypeName);
            var type = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            dynamic dict = Activator.CreateInstance(type, true);
            foreach (var element in elements)
            {
                dict.Add(GetValue(element["Key"]), GetValue(element["Value"]));
            }
            return dict;
        }
        public static object GetDynamicValue(List<XmlElement> elements)
        {
            var dict = new DynamicDictionary();
            foreach (var element in elements)
            {
                dict.Dictionary.Add(GetValue(element["Key"]), GetValue(element["Value"]));
            }
            return dict;
        }
        public static object GetCustomValue(string assemblyFullName, string typeFullName, List<XmlElement> elements)
        {
            var type = Assembly.Load(assemblyFullName).GetType(typeFullName);
            if (type == null)
                return null;
            dynamic obj = Activator.CreateInstance(type, true);

            foreach (var element in elements)
            {
                var property = type.GetProperty(element.Name);
                object value;
                if (!CoralConvert.Convert(GetValue(element), property.PropertyType, out value))
                    continue;
                property.SetValue(obj, value);
            }

            return obj;
        }
        #endregion

        #region 创建XmlElement

        /// <summary>
        /// 创建xml元素
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static XmlElement GetXml(string name, object data, XmlDocument doc)
        {
            if (data == null)
                return null;
            if (data.GetType().IsValueType || data is string)
            {
                return GetValueTypeXml(name, data, doc);
            }
            var list = data as IList;
            if (list != null)
            {
                return GetIListXml(name, list, doc);
            }
            var dict = data as IDictionary;
            if (dict != null)
            {
                return GetIDictionaryXml(name, dict, doc);
            }
            var dynamic = data as DynamicDictionary;
            if (dynamic != null)
            {
                return GetDynamicXml(name, dynamic, doc);
            }
            return GetCustomXml(name, data, doc);
        }

        /// <summary>
        /// 创建简单类型的xml元素
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static XmlElement GetValueTypeXml(string name, object data, XmlDocument doc)
        {
            if (data == null)
                return null;
            var element = doc.CreateElement(name);
            element.SetAttribute("Type", data.GetType().FullName);
            element.SetAttribute("Assembly", MetaDataManager.Assembly.GetAssemblySortName(data.GetType().Assembly));
            element.SetAttribute("Classify", Classify.Sample.ToString());
            element.InnerText = data.ToString();
            return element;
        }

        /// <summary>
        /// 获取列表类型的xml
        /// </summary>
        /// <param name="name"></param>
        /// <param name="datas"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static XmlElement GetIListXml(string name, object datas, XmlDocument doc)
        {
            if (datas == null)
                return null;
            var element = doc.CreateElement(name);
            if (datas.GetType().IsArray)
            {
                element.SetAttribute("Type", typeof(Array).FullName);
                element.SetAttribute("Classify", Classify.Array.ToString());
                element.SetAttribute("ElementType", datas.GetType().GetElementType().FullName);
                element.SetAttribute("ElementAssembly", datas.GetType().GetElementType().Assembly.FullName);
            }
            else
            {
                element.SetAttribute("Type", typeof(IList).FullName);
                element.SetAttribute("Classify", Classify.List.ToString());
                element.SetAttribute("GenericType", datas.GetType().GenericTypeArguments[0].FullName);
                element.SetAttribute("GenericAssembly", datas.GetType().GenericTypeArguments[0].Assembly.FullName);
            }
            foreach (var data in (IList)datas)
            {
                element.AppendChild(GetXml("Element", data, doc));
            }
            return element;
        }

        /// <summary>
        /// 创建动态类型的xml
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static XmlElement GetDynamicXml(string name, dynamic data, XmlDocument doc)
        {
            if (data == null)
                return null;
            var element = doc.CreateElement(name);
            element.SetAttribute("Type", "dynamic");
            element.SetAttribute("Classify", Classify.Dynamic.ToString());
            foreach (DictionaryEntry item in (IDictionary)data.Dictionary)
            {
                var child = doc.CreateElement("Element");
                child.AppendChild(GetXml("Key", item.Key ?? string.Empty, doc));
                child.AppendChild(GetXml("Value", item.Value ?? string.Empty, doc));
                element.AppendChild(child);
            }
            return element;
        }

        /// <summary>
        /// 创建字典类型的xml
        /// </summary>
        /// <param name="name"></param>
        /// <param name="datas"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static XmlElement GetIDictionaryXml(string name, object datas, XmlDocument doc)
        {
            if (datas == null)
                return null;
            var element = doc.CreateElement(name);
            element.SetAttribute("Type", typeof(IDictionary).FullName);
            element.SetAttribute("Classify", Classify.Dictionary.ToString());
            element.SetAttribute("KeyGenericAssembly", datas.GetType().GetGenericArguments()[0].Assembly.FullName);
            element.SetAttribute("KeyGenericType", datas.GetType().GetGenericArguments()[0].FullName);
            element.SetAttribute("ValueGenericAssembly", datas.GetType().GetGenericArguments()[1].Assembly.FullName);
            element.SetAttribute("ValueGenericType", datas.GetType().GetGenericArguments()[1].FullName);
            foreach (DictionaryEntry data in (IDictionary)datas)
            {
                var child = doc.CreateElement("Element");
                child.AppendChild(GetXml("Key", data.Key ?? string.Empty, doc));
                child.AppendChild(GetXml("Value", data.Value ?? string.Empty, doc));
                element.AppendChild(child);
            }
            return element;
        }

        /// <summary>
        /// 创建自定义类
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static XmlElement GetCustomXml(string name, object data, XmlDocument doc)
        {
            if (data == null)
                return null;
            var element = doc.CreateElement(name);
            element.SetAttribute("Assembly",MetaDataManager.Assembly.GetAssemblySortName(data.GetType().Assembly));
            element.SetAttribute("Type", data.GetType().FullName);
            element.SetAttribute("Classify", Classify.Custom.ToString());
            data.GetType().GetProperties().ForEach(property =>
            {
                var item = GetXml(property.Name, property.GetValue(data), doc);
                if (item != null)
                    element.AppendChild(item);
            });
            return element;
        }

        #endregion

        public enum Classify
        {
            Sample,
            List,
            Array,
            Dictionary,
            Dynamic,
            Custom,
        }
        public static List<XmlElement> GetChidlren(this XmlElement element)
        {
            return element.Cast<XmlElement>().ToList();
        }
    }
}
