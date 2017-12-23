using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Models;
using Newtonsoft.Json;

namespace Coralcode.Framework.MetaData
{
    public class MetaDataAttribute : DescriptionAttribute
    {
        public MetaDataAttribute(string description)
            :base(description)
        {
           
        }

        public new string Description
        {
            get { return base.DescriptionValue; } 
        }

        public static MetaDataModel ToMetaData(Type type)
        {
            var attr = type.GetCustomAttribute<MetaDataAttribute>();
            return new MetaDataModel() { CnName = attr.Description, EnName = type.Name };
        }
        public static MetaDataModel ToMetaDataWithFullName(Type type)
        {
            var attr = type.GetCustomAttribute<MetaDataAttribute>();
            return new MetaDataModel() { CnName = attr.Description, EnName = type.FullName };
        }

        public static List<MetaDataModel> GetPropertiesMetaDatas(Type type)
        {
            return type.GetProperties().Select(ToMetaData).Where(item => item != null).ToList();
        }
        public static MetaDataModel ToMetaData(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<MetaDataAttribute>();
            if (attr == null)
                return null;

            return new MetaDataModel
            {
                EnName = property.Name,
                CnName = attr.Description,
                DisplayType = GetDisplay(property.PropertyType),
                DataSource = GetSource(property.PropertyType)
            };
        }

        private static string GetSource(Type type)
        {
            if (type != typeof (Enum) && type.BaseType != typeof (Enum)) return string.Empty;

            return type.GetDescriptionToDataSourceJson();

        }

        private static string GetDisplay(Type type)
        {

            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return "时间";
            }

            if (type == typeof(int) || type == typeof(int?) || type == typeof(decimal) || type == typeof(decimal?) || type == typeof(double) || type == typeof(double?))
            {
                return "值类型";
            }

            if (type == typeof(Enum) || type.BaseType == typeof(Enum))
            {
                return "枚举";
            }

            return "文本";
        }
    }

    public class MetaDataModel
    {
        public string EnName { get; set; }
        public string CnName { get; set; }
        public string DisplayType { get; set; }

        public string DataSource { get; set; }
    }
}
