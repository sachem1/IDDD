using Coralcode.Framework.Data.Specification;

namespace Coralcode.Framework.Data.Core
{
    /// <summary>
    /// 包含查询和操作的仓储
    /// </summary>
    /// <typeparam name="TEntity">Type of entity for this repository </typeparam>
    /// <typeparam name="TSpecification"></typeparam>
    public interface IRepository<TEntity, in TSpecification> : ICommandRepository<TEntity>,IQueryRepository<TEntity,TSpecification> 
        where TEntity :class ,IEntity 
        where TSpecification : ISpecification<TEntity>
    {
    }
    
}
