using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Coralcode.Framework.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// 扩展表达式排序功能，主要用于在数据库中，
        /// 由于属性是string类型这样限制了此功能
        /// 有待后续采取更好的解决方案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="orderByExpressions"> 排序表达式的列表，</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> Orderby<T>(this IQueryable<T> items, List<KeyValuePair<Expression<Func<T, string>>, bool>> orderByExpressions)
        {
            var item = orderByExpressions[0];
            var datas = item.Value ? items.OrderBy(item.Key) : items.OrderByDescending(item.Key);
            for (var i = 1; i < orderByExpressions.Count; i++)
            {
                datas = item.Value ? datas.ThenBy(item.Key) : datas.OrderByDescending(item.Key);
            }
            return datas;
        }

        /// <summary>
        /// 查找对象的位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> items, Expression<Func<T, bool>> expression)
        {
            var list = items.ToList();
            var item = list.FirstOrDefault(expression.Compile());
            if (item == null)
                return -1;
            return list.IndexOf(item);
        }

        /// <summary>
        /// 扩展List的ForEach到IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
        
        public static TProperty PropertyValue<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> errorProperty)
            where TEntity : new()
        {
            var entity = Activator.CreateInstance<TEntity>();
            return errorProperty.Compile()(entity);
        }

        public static Expression<Func<TEntity, dynamic>> CreateFuncExpression<TEntity>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "item");
            //根据属性名获取属性 
            var property = typeof(TEntity).GetProperty(propertyName);
            //创建一个访问属性的表达式 item.property
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            //创建表达式 item=>item.property
            return Expression.Lambda<Func<TEntity, dynamic>>(propertyAccess, parameter);
        }


        /// <summary>
        /// 去重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

    }
}
