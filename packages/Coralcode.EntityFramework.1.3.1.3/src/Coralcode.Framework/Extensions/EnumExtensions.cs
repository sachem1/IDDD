using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Models;
using Newtonsoft.Json;

namespace Coralcode.Framework.Extensions
{

    public class EnumDescription
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public string Description { get; set; }
    }

    public static class EnumExtensions
    {
        public static List<EnumDescription> GetEnumDescriptions(this Type eEnum)
        {
            return eEnum.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(item => item.IsStatic).Select(field =>
                    new EnumDescription()
                    {
                        Description = field.GetDescription(),
                        Name = field.Name,
                        Value = (int)Enum.Parse(eEnum,field.Name)
                    }
                ).ToList();
        }

        public static List<DataSourceModel> GetDescriptionToDataSource(this Type eEnum)
        {
            var selectItem = new List<DataSourceModel>();
            var descriptios = eEnum.GetEnumDescriptions();
            descriptios.ForEach(item =>
            {
                selectItem.Add(new DataSourceModel() { Text = item.Description, Value = item.Value.ToString() });
            });
            return selectItem;
        }

        public static string GetDescriptionToDataSourceJson(this Type eEnum)
        {
            return JsonConvert.SerializeObject(GetDescriptionToDataSource(eEnum));
        }

        public static string GetDescription(this Type type, dynamic value)
        {
            return type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(item => item.Name == value.ToString()
                    || Enum.Parse(type, item.Name).Equals(value)
                    || ((int)Enum.Parse(type, item.Name)).Equals(value)
                    )
                .GetDescription();
        }

        public static string GetDescriptionByField<T>(this T value)where T:struct 
        {
            return typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public)
                .First(item =>item.Name == value.ToString())
                .GetDescription();
        }

        public static dynamic GetEnumValueByDescription(this Type eEnum, string description)
        {
            var field = eEnum.GetFields(BindingFlags.Static | BindingFlags.Public)
                .First(item => item.GetDescription() == description);
            return Enum.Parse(eEnum, field.Name);
        }
        
    }
}
