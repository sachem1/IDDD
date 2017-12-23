using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.Specification;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Models;
using Coralcode.Framework.Page;

namespace Coralcode.Framework.Data.DynamicRepository
{
    public abstract class BaseDynamicQueryRepository<TEntity> : IDynamicQueryRepository<TEntity>
        where TEntity : class, IEntity, IDynamicRouter
    {
        protected DbFactory DbFactory;

        protected BaseDynamicQueryRepository(DbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }
        public abstract void Dispose();

        /// <summary>
        /// 根据规约获取第一个实体
        /// </summary>
        /// <param name="specification"></param>
        /// <returns>实体或者null</returns>
        public TEntity GetFirst(IDynamicSpecification<TEntity> specification)
        {
            return DynamicGetAll(specification).FirstOrDefault(specification.SatisfiedBy());
        }

        /// <summary>
        /// 获取单个实体
        /// 如果存在多个会给出异常
        /// </summary>
        /// <param name="specification"></param>
        /// <returns>实体或者null</returns>
        public TEntity GetSingle(IDynamicSpecification<TEntity> specification)
        {
            return DynamicGetAll(specification).SingleOrDefault(specification.SatisfiedBy());
        }

        /// <summary>
        /// 获取最后一个实体
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        public TEntity GetLast(IDynamicSpecification<TEntity> specification)
        {
            return DynamicGetAll(specification).LastOrDefault(specification.SatisfiedBy());
        }

        /// <summary>
        /// 获取实体的个数
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        public long GetCount(IDynamicSpecification<TEntity> specification)
        {
            return DynamicGetAll(specification).Count(specification.SatisfiedBy());
        }

        /// <summary>
        ///获取所有的实体
        /// </summary>
        /// <returns>List of selected elements</returns>
        public IQueryable<TEntity> GetAll()
        {
            return DbFactory.GetDynamicDbConfigs(typeof(TEntity)).SelectMany(item => DynamicGetAll(new SampleRouter(item.DynamicCoden)).ToList()).AsQueryable();
        }

        /// <summary>
        /// 动态获取所有
        /// </summary>
        /// <returns></returns>
        protected abstract IQueryable<TEntity> DynamicGetAll(IDynamicRouter router);

        /// <summary>
        /// 根据规约查询
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetAllMatching(IDynamicSpecification<TEntity> specification)
        {
            return DynamicGetAll(specification).Where(specification.SatisfiedBy());
        }

        /// <summary>
        ///获取分页数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageCount">页大小</param>
        /// <param name="specification">条件</param>
        /// <param name="orderByExpressions">是否排序</param>
        /// <returns>实体的分页数据</returns>
        public PagedList<TEntity> GetPaged(int pageIndex, int pageCount, IDynamicSpecification<TEntity> specification,
            SortExpression<TEntity> orderByExpressions = null)
        {
            if (orderByExpressions == null || !orderByExpressions.IsNeedSort())
                orderByExpressions = new SortExpression<TEntity>(new List<EditableKeyValuePair<Expression<Func<TEntity, dynamic>>, bool>>
                {
                    new EditableKeyValuePair<Expression<Func<TEntity, dynamic>>, bool>(item=>item.Id,false),
                });

            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            //如果动态路由可用则为单库
            if (!string.IsNullOrEmpty(specification.Coden))
            {
                var set = DynamicGetAll(specification);
                //如果找到了单库
                if (set != null)
                {
                    var queryable = set.Where(specification.SatisfiedBy());
                    int totel = queryable.Count();
                    IEnumerable<TEntity> items = orderByExpressions.BuildSort(queryable).Skip(pageCount * (pageIndex - 1)).Take(pageCount);
                    return new PagedList<TEntity>(totel, pageCount, pageIndex, items.ToList());
                }

            }
           
            //如果找不到单库
            int sum = 0;
            List<IQueryable<TEntity>> entities = new List<IQueryable<TEntity>>();
            foreach (var tmp in DbFactory.GetDynamicDbConfigs(typeof(TEntity)))
            {
                var queryable = DynamicGetAll(new SampleRouter(tmp.DynamicCoden)).Where(specification.SatisfiedBy());
                sum += queryable.Count();
                entities.Add(queryable);
            }
            int newDataIndex = (pageIndex + 1) * pageCount;
            //如果在中值之后则反转排序
            if (sum < pageIndex * pageCount * 2 && pageIndex * pageCount > sum)
            {
                orderByExpressions.Reverse();
                //反转页码
                newDataIndex = sum - pageIndex * pageCount;
                var datas = entities.SelectMany(item => orderByExpressions.BuildSort(item).Take(newDataIndex)).ToList();

                orderByExpressions.Reverse();
                datas = orderByExpressions.BuildSort(datas).Skip(0).Take(pageCount).ToList();
                return new PagedList<TEntity>(sum, pageCount, pageIndex, datas.ToList());
            }
            else
            {
                var datas = entities.SelectMany(item => orderByExpressions.BuildSort(item).Take(newDataIndex))
                    .Skip(pageCount * (pageIndex - 1)).Take(pageCount).ToList();
                return new PagedList<TEntity>(sum, pageCount, pageIndex, datas.ToList());
            }
        }

        public List<TEntity> GetByIds(List<long> ids)
        {
            return ids.GroupBy(id => DbFactory.AnalysisNumber(id))
                           .SelectMany(items =>
                            {
                                var config = DbFactory.GetDynamicDbConfigByNumber(typeof(TEntity), items.Key);
                                return DynamicGetAll(new SampleRouter(config.DynamicCoden))
                                    .Where(data => items.Contains(data.Id));
                            }).ToList();
        }
    }
}
