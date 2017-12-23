using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using Coralcode.Framework.Common;
using Newtonsoft.Json;

namespace Coralcode.Framework.Extensions
{
    public static class EntityExtensions
    {

        /// <summary>
        /// 根据属性获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetValueByPropery<T>(this T entity, string propertyName) where T : class
        {
            var property = typeof(T).GetProperties().FirstOrDefault(item => item.Name == propertyName);
            if (property == null)
                return null;
            return property.GetValue(entity, null);
        }
        /// <summary>
        /// 根据属性设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetValueByPropery<T>(this T entity, string propertyName, object value) where T : class
        {
            var property = typeof(T).GetProperties().FirstOrDefault(item => item.Name == propertyName);
            if (property == null)
                return null;
            property.SetValue(entity, CoralConvert.Convert(value, property.PropertyType));
            return property;
        }

        public static PropertyInfo GetProperyByName<T>(this T entity, string propertyName) where T : class
        {
            var property = typeof(T).GetProperties().FirstOrDefault(item => item.Name == propertyName);
            return property;
        }



        public static void BubbleSort<T>(this List<T> items, Func<T, T, bool> compareAction)
        {
            var count = items.Count;
            for (int i = 0; i < count; i++)
            {
                for (int j = i; j < count; j++)
                {
                    if (compareAction(items[j], items[i]))
                    {
                        var c = items[i];
                        items[i] = items[j];
                        items[j] = c;
                    }
                }
            }
        }


        /// <summary>
        /// 获取枚举中的自定义描述List<long, string>
        /// </summary>
        /// <typeparam name="TAttr">自定义Attribute</typeparam>
        /// <param name="type">枚举类类型</param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetPropertyAndDescriptings(this Type type)
        {
            return type.GetProperties()
               .ToDictionary(item => item.Name, item => item.GetDescription())
               .Where(item => !string.IsNullOrEmpty(item.Key))
               .ToList();
        }

        public static T DeepClone<T>(this T entity) where T : class, new()
        {
            if (entity == null)
                return default(T);
            var json = JsonConvert.SerializeObject(entity);
            return JsonConvert.DeserializeObject<T>(json);
        }
        
        public static T ShallowClone<T>(this T entity) where T : class,ICloneable, new()
        {
            if (entity == null)
                return default(T);
           
            return entity.Clone() as T;
        }


        #region 反射
        /// <summary>
        /// 获取私有字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="fieldname"></param>
        /// <returns></returns>
        public static T GetPrivateField<T>(this object instance, string fieldname)
        {
            var type = instance.GetType();
            var field = type.GetField(fieldname, BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)field.GetValue(instance);
        }


        /// <summary>
        /// 设置私有字段
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="fieldname"></param>
        /// <param name="value"></param>
        public static void SetPrivateField(this object instance, string fieldname, object value)
        {
            var type = instance.GetType();
            var field = type.GetField(fieldname, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(instance, value);
        }
        /// <summary>
        /// 获取私有属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        public static T GetPrivateProperty<T>(this object instance, string propertyname)
        {
            var type = instance.GetType();
            var field = type.GetProperty(propertyname, BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)field.GetValue(instance, null);
        }

        /// <summary>
        /// 设置私有属性
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyname"></param>
        /// <param name="value"></param>
        public static void SetPrivateProperty(this object instance, string propertyname, object value)
        {
            var type = instance.GetType();
            var field = type.GetProperty(propertyname, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(instance, value, null);
        }

        /// <summary>
        /// 执行方法 公有/私有
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T ExecuteMethod<T>(this object instance, string name, params object[] param)
        {
            var type = instance.GetType();
            var method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic|BindingFlags.Public);
            return (T)method.Invoke(instance, param);
        }

        /// <summary>
        /// 执行父类的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="parentType"></param>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T ExecuteParentMethod<T>(this object instance,Type parentType, string name, params object[] param)
        {
            var method = parentType.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            return (T)method.Invoke(instance, param);
        }
        #endregion

    }
}
