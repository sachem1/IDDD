using System;
using System.Collections.Generic;
using Coralcode.Framework.Data.Specification;
using Coralcode.Framework.Page;
using System.Linq;

namespace Coralcode.Framework.Data.Core
{
    /// <summary>
    /// 只提供查询的仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TSpecification"></typeparam>
    public interface IQueryRepository<TEntity, in TSpecification> : IDisposable
        where TEntity : class, IEntity
        where TSpecification : ISpecification<TEntity>
    {

        /// <summary>
        /// 根据规约获取第一个实体
        /// </summary>
        /// <param name="specification"></param>
        /// <returns>实体或者null</returns>
        TEntity GetFirst(TSpecification specification);

        /// <summary>
        /// 获取单个实体
        /// 如果存在多个会给出异常
        /// </summary>
        /// <param name="specification"></param>
        /// <returns>实体或者null</returns>
        TEntity GetSingle(TSpecification specification);


        /// <summary>
        /// 获取最后一个实体
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        TEntity GetLast(TSpecification specification);

        /// <summary>
        /// 获取实体的个数
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        long GetCount(TSpecification specification);

        /// <summary>
        ///获取所有的实体
        /// </summary>
        /// <returns>List of selected elements</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// 根据规约查询
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetAllMatching(TSpecification specification);

        /// <summary>
        ///获取分页数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageCount">页大小</param>
        /// <param name="specification">条件</param>
        /// <param name="orderByExpressions">是否排序</param>
        /// <returns>实体的分页数据</returns>
        PagedList<TEntity> GetPaged(int pageIndex, int pageCount, TSpecification specification, SortExpression<TEntity> orderByExpressions = null);
    }
}
