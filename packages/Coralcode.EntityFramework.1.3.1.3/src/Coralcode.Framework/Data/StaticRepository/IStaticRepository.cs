using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.Specification;

namespace Coralcode.Framework.Data.StaticRepository
{
    /// <summary>
    /// 包含查询和操作的仓储
    /// </summary>
    /// <typeparam name="TEntity">Type of entity for this repository </typeparam>
    public interface IStaticRepository<TEntity> : 
        IStaticCommandRepository<TEntity>, 
        IStaticQueryRepository<TEntity> ,
        IRepository<TEntity,ISpecification<TEntity>>
        where TEntity : class, IEntity
    {
    }

}
