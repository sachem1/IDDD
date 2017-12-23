using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coralcode.Framework.Extensions;

namespace Coralcode.Framework.Common
{
    public class CoralConvert
    {
        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="source">源值</param>
        /// <param name="conversionType">类型</param>
        /// <param name="dest">目标值</param>
        /// <returns></returns>
        public static bool Convert(object source, Type conversionType, out dynamic dest)
        {
            dest = null;
            if (source == null)
            {
                return false;
            }

            var strSource = source.ToString();
            if (string.IsNullOrEmpty(strSource) && conversionType != typeof(string))
            {
                return false;
            }

            if (conversionType.IsGenericType 
                && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                conversionType = Nullable.GetUnderlyingType(conversionType);
            }


            if (conversionType == typeof(DateTime) || conversionType == typeof(DateTime?))
            {
                DateTime dateTimeValue;
                if (!DateTime.TryParse(strSource, out dateTimeValue))
                {
                    return false;
                }

                dest = dateTimeValue;
                return true;
            }
            if (conversionType == typeof(decimal) || conversionType == typeof(decimal?))
            {
                decimal decimalValue;
                if (!decimal.TryParse(strSource, out decimalValue))
                {
                    return false;
                }
                dest = decimalValue;
                return true;
            }

            if (conversionType == typeof(int) || conversionType == typeof(int?))
            {
                int intValue;
                if (!int.TryParse(strSource, out intValue))
                    return false;
                dest = intValue;
                return true;
            }
            if (conversionType == typeof(double) || conversionType == typeof(double?))
            {
                double doubleValue;
                if (!double.TryParse(strSource, out doubleValue))
                    return false;
                dest = doubleValue;
                return true;
            }
            if (conversionType == typeof(string))
            {
                dest = strSource;
                return true;
            }
            if (conversionType == typeof(Enum) || conversionType.BaseType == typeof(Enum))
            {
                var enumValueDescs = conversionType.GetEnumDescriptions();
                foreach (var valueDesc in enumValueDescs)
                {
                    if (strSource.Equals(valueDesc.Name)
                        || strSource.Equals(valueDesc.Value.ToString())
                        || strSource.Equals(valueDesc.Description))
                    {
                        dest = Enum.Parse(conversionType, valueDesc.Name);
                        return true;
                    }
                }

                return false;
            }
            if (source.GetType().IsAssignableFrom(conversionType)
                ||source.GetType().IsSubclassOf(conversionType)
                || source.GetType().GetInterfaces().Contains(conversionType))
            {

                dest = source;
                return true;
            }

            try
            {
                dest = System.Convert.ChangeType(source, conversionType);
                return true;
            }
            catch
            {
                return false;
            }


        }


        public static T Convert<T>(object source)
        {
            object dest;
            if (Convert(source, typeof(T), out dest))
                return (T)dest;
            return default(T);
        }

        public static object Convert(object source, Type conversionType)
        {
            object dest;
            if (Convert(source, conversionType, out dest))
                return dest;
            return null;
        }

        public static void Convert<T>(Dictionary<string, object> source, T model, params string[] ignoreMembers) where T : new()
        {
            if(source == null || source.Count < 1)
                throw new ArgumentException(@"参数不能为空",nameof(source));

            var modelType = model.GetType();
            var modelProperties =
                modelType.GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                        .ToList();

            if (modelProperties.Count < 1)
                return ;

            foreach (PropertyInfo modelProperty in modelProperties)
            {
                var memberName = modelProperty.Name;

                if (ignoreMembers.Contains(memberName))
                    continue;

                if (!source.ContainsKey(memberName))
                {
                    continue;
                }

                var value = Convert(source[memberName], modelProperty.PropertyType);
                if (value == null)
                    continue;

                modelProperty.SetValue(model,value);

            }

            return;
        }

        public static T Convert<T>(Dictionary<string, object> source, params string[] ignoreMembers) where T : new()
        {
            T model = new T();
            Convert(source, model, ignoreMembers);
            return model;
        }

        public static object Default<T>()
        {
            return default(T);
        }

        public static object Default(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

    }
}