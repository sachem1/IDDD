using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Coralcode.Framework.Extensions
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// 将一个列表转换成DataTable,如果列表为空将返回空的DataTable结构
        /// </summary>
        /// <typeparam name="T">要转换的数据类型</typeparam>
        /// <param name="dt"></param>
        /// <param name="entityList">实体对象列表</param> 
        public static DataTable Import<T>(this DataTable dt, IList<T> entityList)
        {

            //取类型T所有Propertie
            Type entityType = typeof(T);
            PropertyInfo[] entityProperties = entityType.GetProperties();
            Type colType = null;
            foreach (PropertyInfo propInfo in entityProperties)
            {

                if (propInfo.PropertyType.IsGenericType)
                {
                    colType = Nullable.GetUnderlyingType(propInfo.PropertyType);
                }
                else
                {
                    colType = propInfo.PropertyType;
                }

                if (colType.FullName.StartsWith("System"))
                {
                    dt.Columns.Add(propInfo.Name, colType);
                }
            }

            if (entityList != null && entityList.Count > 0)
            {
                foreach (T entity in entityList)
                {
                    DataRow newRow = dt.NewRow();
                    foreach (PropertyInfo propInfo in entityProperties)
                    {
                        if (dt.Columns.Contains(propInfo.Name))
                        {
                            object objValue = propInfo.GetValue(entity, null);
                            newRow[propInfo.Name] = objValue ?? DBNull.Value;
                        }
                    }
                    dt.Rows.Add(newRow);
                }
            }

            return dt;
        }

        /// <summary>
        /// 将一个DataTable转换成列表
        /// </summary>
        /// <typeparam name="T">实体对象的类型</typeparam>
        /// <param name="dt">要转换的DataTable</param>
        /// <returns></returns>
        public static List<T> ToEntityList<T>(this DataTable dt)
        {
            List<T> entiyList = new List<T>();

            Type entityType = typeof(T);
            PropertyInfo[] entityProperties = entityType.GetProperties();

            foreach (DataRow row in dt.Rows)
            {
                T entity = Activator.CreateInstance<T>();

                foreach (PropertyInfo propInfo in entityProperties)
                {
                    if (dt.Columns.Contains(propInfo.Name))
                    {
                        if (!row.IsNull(propInfo.Name))
                        {
                            propInfo.SetValue(entity, row[propInfo.Name], null);
                        }
                    }
                }

                entiyList.Add(entity);
            }

            return entiyList;
        }

        public static List<Dictionary<string, object>> ToDictionary(this DataTable dt)
        {
            var dictList = new List<Dictionary<string, object>>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var dict = new Dictionary<string, object>();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    dict.Add(dt.Columns[j].ColumnName, Convert.ToString(dt.Rows[i][j].ToString()));
                }
                dictList.Add(dict);
            }
            return dictList;
        }
        private static string ToStr(object s, string format = "")
        {
            string result = "";
            try
            {
                if (format == "")
                {
                    result = s.ToString();
                }
                else
                {
                    result = string.Format("{0:" + format + "}", s);
                }
            }
            catch
            {
            }
            return result;
        }


        public static DataTable Merge(this IEnumerable<DataTable> dts)
        {
            return dts.Aggregate((current, next) =>
             {
                 current.Merge(next);
                 return current;
             });
        }

        /// <summary>
        /// 增加一列
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="column"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable AddColumn(this DataTable dt, string column, Type type)
        {
            dt.Columns.Add(new DataColumn(column, type));
            return dt;
        }

    }
}
