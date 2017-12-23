using System.Collections.Generic;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Extensions;

namespace Coralcode.Framework.Data.DynamicRepository
{
    /// <summary>
    /// 包含查询和操作的仓储
    /// </summary>
    /// <typeparam name="TEntity">Type of entity for this repository </typeparam>
    public abstract class BaseDynamicRepository<TEntity> : BaseDynamicQueryRepository<TEntity>, IDynamicRepository<TEntity>
        where TEntity : class, IEntity, IDynamicRouter
    {
        protected BaseDynamicRepository(DbFactory dbFactory) : base(dbFactory)
        {
        }

        /// <summary>
        ///工作单元
        /// </summary>
        public abstract IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="item"></param>
        public abstract void Add(TEntity item);
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<TEntity> items)
        {
            items.ForEach(Add);
        }

        /// <summary>
        /// 删除实体 
        /// </summary>
        /// <param name="item"></param>
        public abstract void Remove(TEntity item);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="items"></param>
        public void RemoveRange(IEnumerable<TEntity> items)
        {
            items.ForEach(Remove);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="ids"></param>
        public void RemoveRange(IEnumerable<long> ids)
        {
            ids.ForEach(Remove);
        }

        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <param name="id">主键</param>
        public void Remove(long id)
        {
            Remove(Get(id));
        }

        /// <summary>
        /// 更改实体
        /// </summary>
        /// <param name="item"></param>
        public abstract void Modify(TEntity item);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="items"></param>
        public void ModifyRange(IEnumerable<TEntity> items)
        {
            items.ForEach(Modify);
        }

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract TEntity Get(long id);
    }

}
