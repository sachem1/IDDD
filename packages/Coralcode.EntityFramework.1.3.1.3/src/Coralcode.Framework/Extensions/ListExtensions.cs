using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Coralcode.Framework.Common;
using Coralcode.Framework.Models;
using Coralcode.Framework.Services;
using NPOI.SS.Formula.Functions;

namespace Coralcode.Framework.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// 把list转换成数据表,
        ///  todo 这里如果属性是类，需要从类里面取一个字段作为值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static DataTable Export<T>(this List<T> entities) where T : class
        {
            var dt = new DataTable();
            var properties = typeof(T).GetProperties().ToList();
            var fullName = new List<string>();
            properties.ForEach(
                item =>
                {

                    var tempType = item.PropertyType;

                    if (item.PropertyType.IsGenericType &&
                        item.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        // 存在可空类型
                        tempType = Nullable.GetUnderlyingType(item.PropertyType);
                        fullName.Add(item.PropertyType.FullName);
                    }

                    if (tempType == typeof(Enum) || tempType.BaseType == typeof(Enum))
                    {
                        tempType = typeof(int);
                    }

                    dt.Columns.Add(new DataColumn(item.Name) { DataType = tempType });

                });

            entities.ToList().ForEach(item =>
            {
                var dr = dt.NewRow();
                properties.ForEach(
                    property =>
                    {
                        object value;
                        if (fullName.Any(r => r == property.PropertyType.FullName))
                        {
                            value = property.GetValue(item, null) ?? DBNull.Value;
                        }
                        else
                        {
                            value = property.GetValue(item, null);
                            ConvertValue(property, value);
                        }
                        dr[property.Name] = value;

                    });
                dt.Rows.Add(dr);
            });
            return dt;
        }

        private static void ConvertValue(PropertyInfo property, object value)
        {
            if (property.PropertyType == typeof(DateTime) && (DateTime)value == default(DateTime))
            {
                value = default(DateTime);
                //new DateTime(1753, 1, 1);
            }
        }

        /// <summary>
        /// 把数据表转换成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> Import<T>(this List<T> list, DataTable dt, out List<ImportMessage> errorMessages) where T : class,new()
        {
            var plist = new List<PropertyInfo>(typeof(T).GetProperties());
            errorMessages = new List<ImportMessage>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow item = dt.Rows[i];
                var s = Activator.CreateInstance<T>();
                foreach (DataColumn column in dt.Columns)
                {
                    var info = plist.FirstOrDefault(p => p.Name == column.ColumnName);
                    if (info == null) continue;
                    if (item[column.ColumnName] == null)
                        continue;

                    dynamic dest;
                    var isConvert = false;
                    try
                    {
                        isConvert = CoralConvert.Convert(item[column.ColumnName], info.PropertyType, out dest);
                    }
                    catch (Exception ex)
                    {
                        errorMessages.Add(new ImportMessage
                        {
                            Index = i,
                            State = ResultState.Fail,
                            ErrorMessage = string.Format("{0}的值：{1} 类型转换失败,{2}", column.ColumnName, item[column.ColumnName], ex.Message)
                        });
                        continue;
                    }

                    if (!isConvert)
                    {
                        errorMessages.Add(new ImportMessage
                        {
                            Index = i,
                            State = ResultState.Fail,
                            ErrorMessage = string.Format("{0}的值：{1} 类型转换失败", column.ColumnName, item[column.ColumnName])
                        });
                        continue;
                    }
                    info.SetValue(s, dest, null);

                }
                list.Add(s);
            }

            return list;
        }

        /// <summary>
        /// 把list转换成数据表,
        ///  todo 这里如果属性是类，需要从类里面取一个字段作为值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="focusHeader"></param>
        /// <returns></returns>
        public static DataTable ExportWithDescription<T>(this List<T> entities) where T : class
        {
            var dt = new DataTable();
            var fullName = new List<string>();
            var properties = typeof(T).GetProperties().ToList();
            properties.ForEach(item =>
            {
                var des = PropertyExtensions.GetDescription(item);

                var tempType = item.PropertyType;

                if (item.PropertyType.IsGenericType &&
                    item.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    // 存在可空类型
                    tempType = Nullable.GetUnderlyingType(item.PropertyType);
                    fullName.Add(item.PropertyType.FullName);
                }

                if (tempType == typeof(Enum) || tempType.BaseType == typeof(Enum))
                {
                    tempType = typeof(int);
                }

                if (!string.IsNullOrEmpty(des))
                    dt.Columns.Add(new DataColumn(des) { DataType = tempType });
            });
            entities.ToList().ForEach(item =>
            {
                var dr = dt.NewRow();
                properties.ForEach(
                    property =>
                    {
                        var des = PropertyExtensions.GetDescription(property);
                        if (string.IsNullOrEmpty(des))
                            return;

                        object value;
                        if (fullName.Any(r => r == property.PropertyType.FullName))
                        {
                            value = property.GetValue(item, null) ?? DBNull.Value;
                        }
                        else
                        {
                            value = property.GetValue(item, null);
                            ConvertValue(property, value);
                        }
                        dr[des] = value;
                    });
                dt.Rows.Add(dr);
            });
            return dt;
        }

        /// <summary>
        /// 把数据表转换成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="dt"></param>
        /// <param name="errorMessages">错误信息</param>
        /// <returns></returns>
        public static List<T> ImportWithDescription<T>(this List<T> list, DataTable dt, out List<ImportMessage> errorMessages) where T : class,new()
        {
            var plist = new List<PropertyInfo>(typeof(T).GetProperties());

            errorMessages = new List<ImportMessage>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow item = dt.Rows[i];
                var s = Activator.CreateInstance<T>();
                foreach (DataColumn column in dt.Columns)
                {
                    var info = plist.FirstOrDefault(p => PropertyExtensions.GetDescription(p) == column.ColumnName);
                    if (info == null) continue;
                    if (item[column.ColumnName] == null)
                        continue;

                    dynamic dest;
                    var isConvert = false;
                    try
                    {
                        isConvert = CoralConvert.Convert(item[column.ColumnName], info.PropertyType, out dest);
                    }
                    catch (Exception ex)
                    {
                        errorMessages.Add(new ImportMessage
                        {
                            Index = i,
                            State = ResultState.Fail,
                            ErrorMessage = string.Format("{0}的值：{1} 类型转换失败,{2}", column.ColumnName, item[column.ColumnName], ex.Message)
                        });
                        continue;
                    }

                    if (!isConvert)
                    {
                        errorMessages.Add(new ImportMessage
                        {
                            Index = i,
                            State = ResultState.Fail,
                            ErrorMessage = string.Format("{0}的值：{1} 类型转换失败", column.ColumnName, item[column.ColumnName])
                        });
                        continue;
                    }
                    info.SetValue(s, dest, null);

                }
                list.Add(s);
            }

            return list;
        }


        public static List<List<T>> CutToParts<T>(this List<T> list, int everyPartCount)
        {
            int index = 0;
            var result = new List<List<T>>();
            while (index < list.Count)
            {
                result.Add(list.Skip(index).Take(everyPartCount).ToList());
                index += everyPartCount;
            }
            return result;
        }

    }

}
