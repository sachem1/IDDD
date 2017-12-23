using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Coralcode.Framework.Extensions;

namespace Coralcode.Framework.Reflection
{
    public class AttributeFinder
    {
        public TAttribute GetPropertyAttribute<TAttribute>(Type type, string propertyName)
            where TAttribute : Attribute
        {
            return type.GetProperty(propertyName).GetCustomAttribute<TAttribute>();
        }


        /// <summary>
        /// 获取成员特性,如枚举成员
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public TAttribute GetFieldAttribute<TAttribute>(Type type, string fieldName)
            where TAttribute : Attribute
        {
            return type.GetField(fieldName).GetCustomAttribute<TAttribute>();
        }

        #region 枚举
        public List<KeyValuePair<long, string>> GetEnumMemberDescriptions<TEnum>() where TEnum : struct
        {

            return GetEnumMemberDescriptions(typeof(TEnum));
        }

        public List<KeyValuePair<long, string>> GetEnumMemberDescriptions(Type type)
        {
            return type.GetFields()
               .Where(item => item.IsStatic)
               .ToDictionary(item => (long)(int)Enum.Parse(type, item.Name), PropertyExtensions.GetDescription)
               .Where(item => !string.IsNullOrEmpty(item.Value))
               .ToList();
        }


        /// <summary>
        /// 获取枚举中的自定义描述List<long, string>
        /// </summary>
        /// <typeparam name="TAttr">自定义Attribute</typeparam>
        /// <param name="type">枚举类类型</param>
        /// <returns></returns>
        public List<KeyValuePair<long, string>> GetEnumMemberCustomerDescriptions<TAttr>(Type type) where TAttr : Attribute
        {
            return type.GetFields()
               .Where(item => item.IsStatic)
               .ToDictionary(item => (long)(int)Enum.Parse(type, item.Name), item=>item.GetDescription())
               .Where(item => !string.IsNullOrEmpty(item.Value))
               .ToList();
        }


        #endregion
    }
}
