using Coralcode.Framework.Data.Core;

namespace Coralcode.Framework.Data.DynamicRepository
{
    /// <summary>
    /// 包含查询和操作的仓储
    /// </summary>
    /// <typeparam name="TEntity">Type of entity for this repository </typeparam>
    public interface IDynamicRepository<TEntity> : IDynamicCommandRepository<TEntity>,
        IDynamicQueryRepository<TEntity>,
        IRepository<TEntity, IDynamicSpecification<TEntity>>
        where TEntity : class, IEntity, IDynamicRouter
    {
    }

}
