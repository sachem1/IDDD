using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coralcode.Framework.Models;

namespace Coralcode.Framework.Data.Core
{
    public class SortExpression<TEntity> where TEntity : class
    {
        /// <summary>
        /// key:属性名，value,true为升序，false为降序
        /// </summary>
        private readonly List<EditableKeyValuePair<Expression<Func<TEntity, dynamic>>, bool>> _sortList;

        public SortExpression(List<EditableKeyValuePair<Expression<Func<TEntity, dynamic>>, bool>> sortList)
        {
            _sortList = sortList;
        }

        /// <summary>
        /// 如果为空则不需要排序
        /// </summary>
        /// <returns></returns>
        public bool IsNeedSort()
        {
            return _sortList.Count != 0;
        }

        public IQueryable<TEntity> BuildSort(IQueryable<TEntity> query)
        {
            if (_sortList == null || _sortList.Count == 0)
                return query;

            var index = 0;
            _sortList.ForEach(item =>
            {
                //获取表达式变量参数 item
                var parameter = item.Key.Parameters[0];

                //解析属性名
                Expression bodyExpression = null;
                if (item.Key.Body is UnaryExpression)
                {
                    UnaryExpression unaryExpression = item.Key.Body as UnaryExpression;
                    bodyExpression = unaryExpression.Operand;
                }
                else if (item.Key.Body is MemberExpression)
                {
                    bodyExpression = item.Key.Body;
                }
                else
                    throw new ArgumentException(@"The body of the sort predicate expression should be either UnaryExpression or MemberExpression.", "sortPredicate");
                MemberExpression memberExpression = (MemberExpression)bodyExpression;
                string propertyName = memberExpression.Member.Name;

                //根据属性名获取属性 
                var property = typeof(TEntity).GetProperty(propertyName);

                //创建一个访问属性的表达式 item.property
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);

                //创建表达式 item=>item.property
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                string sortMethod = string.Empty;
                if (index == 0)
                    sortMethod = item.Value ? "OrderBy" : "OrderByDescending";
                else
                    sortMethod = item.Value ? "ThenBy" : "ThenByDescending";
                var resultExp = Expression.Call(typeof(Queryable),
                    sortMethod,
                    new[] { typeof(TEntity), property.PropertyType }, query.Expression, Expression.Quote(orderByExp));
                query = query.Provider.CreateQuery<TEntity>(resultExp);
                index++;
            });
            return query;
        }


        public IEnumerable<TEntity> BuildSort(IEnumerable<TEntity> query)
        {
            if (_sortList == null || _sortList.Count == 0)
                return query;
            _sortList.ForEach(item =>
            {
                //获取表达式变量参数 item
                var parameter = item.Key.Parameters[0];

                //解析属性名
                Expression bodyExpression = null;
                if (item.Key.Body is UnaryExpression)
                {
                    UnaryExpression unaryExpression = item.Key.Body as UnaryExpression;
                    bodyExpression = unaryExpression.Operand;
                }
                else if (item.Key.Body is MemberExpression)
                {
                    bodyExpression = item.Key.Body;
                }
                else
                    throw new ArgumentException(@"The body of the sort predicate expression should be either UnaryExpression or MemberExpression.", "sortPredicate");
                MemberExpression memberExpression = (MemberExpression)bodyExpression;
                string propertyName = memberExpression.Member.Name;

                //根据属性名获取属性 
                var property = typeof(TEntity).GetProperty(propertyName);

                //创建一个访问属性的表达式 item.property
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);

                //创建表达式 item=>item.property
                var orderByExp = Expression.Lambda(propertyAccess, parameter);

                //var resultExp = Expression.Call(typeof(IEnumerable),
                //    item.Value ? "OrderBy" : "OrderByDescending",
                //    new[] { typeof(TEntity), property.PropertyType }, query.Expression, Expression.Quote(orderByExp));
                // query =resultExp.Method.Invoke(query,resultExp.Arguments.ToArray())  query.Provider.CreateQuery<TEntity>(resultExp);
            });
            return query;
        }

        public void Reverse()
        {
            _sortList.ForEach(item =>
            {
                item.Value = (!item.Value);
            });
            _sortList.Reverse();
        }
    }
}