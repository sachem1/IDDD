using System.Data.Entity;
using Coralcode.Framework.Data;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.Extension;

namespace Coralcode.EntityFramework.Extension
{
    public interface IEfUnitOfWork<TEntity> : IUnitOfWork
        where TEntity : class, IEntity
    {
        /// <summary>
        /// 创建ef的dbSet 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        DbSet<TEntity> CreateSet(DbConfig config, DbMode mode);

        /// <summary>
        /// 设置更改
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="mode"></param>
        void SetModify(TEntity entity,DbMode mode);
    }
}
