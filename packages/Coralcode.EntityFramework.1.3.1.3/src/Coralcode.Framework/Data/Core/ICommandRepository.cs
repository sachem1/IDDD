using System;
using System.Collections.Generic;

namespace Coralcode.Framework.Data.Core
{
    /// <summary>
    /// 提共增删改的仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ICommandRepository<TEntity> : IDisposable where TEntity : class ,IEntity
    {
        /// <summary>
        ///工作单元
        /// </summary>
        IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="item"></param>
        void Add(TEntity item);


        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="items"></param>
        void AddRange(IEnumerable<TEntity> items);

        /// <summary>
        /// 删除实体 
        /// </summary>
        /// <param name="item"></param>
        void Remove(TEntity item);


        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="items"></param>
        void RemoveRange(IEnumerable<TEntity> items);


        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="ids"></param>
        void RemoveRange(IEnumerable<long> ids);

        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <param name="id">主键</param>
        void Remove(long id);

        /// <summary>
        /// 更改实体
        /// </summary>
        /// <param name="item"></param>
        void Modify(TEntity item);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="items"></param>
        void ModifyRange(IEnumerable<TEntity> items);


        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity Get(long id);
    }
}
