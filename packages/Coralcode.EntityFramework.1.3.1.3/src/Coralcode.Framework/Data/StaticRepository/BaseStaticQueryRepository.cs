using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.Specification;
using Coralcode.Framework.Page;

namespace Coralcode.Framework.Data.StaticRepository
{
    public abstract class BaseStaticQueryRepository<TEntity> : IStaticQueryRepository<TEntity>
        where TEntity : class, IEntity
    {
        public abstract void Dispose();

        /// <summary>
        /// 根据规约获取第一个实体
        /// </summary>
        /// <param name="specification"></param>
        /// <returns>实体或者null</returns>
        public TEntity GetFirst(ISpecification<TEntity> specification)
        {
            return GetAll().FirstOrDefault(specification.SatisfiedBy());
        }

        /// <summary>
        /// 获取单个实体
        /// 如果存在多个会给出异常
        /// </summary>
        /// <param name="specification"></param>
        /// <returns>实体或者null</returns>
        public TEntity GetSingle(ISpecification<TEntity> specification)
        {
            return GetAll().SingleOrDefault(specification.SatisfiedBy());
        }

        /// <summary>
        /// 获取最后一个实体
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        public TEntity GetLast(ISpecification<TEntity> specification)
        {
            return GetAll().LastOrDefault(specification.SatisfiedBy());
        }

        /// <summary>
        /// 获取实体的个数
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        public long GetCount(ISpecification<TEntity> specification)
        {
            return GetAll().Count(specification.SatisfiedBy());
        }

        /// <summary>
        ///获取所有的实体
        /// </summary>
        /// <returns>List of selected elements</returns>
        public abstract IQueryable<TEntity> GetAll();

        /// <summary>
        /// 根据规约查询
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetAllMatching(ISpecification<TEntity> specification)
        {
            return GetAll().Where(specification.SatisfiedBy());
        }

        /// <summary>
        ///获取分页数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageCount">页大小</param>
        /// <param name="specification">条件</param>
        /// <param name="orderByExpressions">是否排序</param>
        /// <returns>实体的分页数据</returns>
        public PagedList<TEntity> GetPaged(int pageIndex, int pageCount, ISpecification<TEntity> specification,
            SortExpression<TEntity> orderByExpressions = null)
        {
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            var set = GetAllMatching(specification);
            int totel = set.Count();
            IEnumerable<TEntity> items;
            if (orderByExpressions != null && orderByExpressions.IsNeedSort())
            {
                items = orderByExpressions.BuildSort(set).Skip(pageCount * (pageIndex - 1)).Take(pageCount);
            }
            else
            {
                items = set.OrderByDescending(item => item.Id).Skip(pageCount * (pageIndex - 1)).Take(pageCount);
            }
            return new PagedList<TEntity>(totel, pageCount, pageIndex, items.ToList());
        }
    }
}
