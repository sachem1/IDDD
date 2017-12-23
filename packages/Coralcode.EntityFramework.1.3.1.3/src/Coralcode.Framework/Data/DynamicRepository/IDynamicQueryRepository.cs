using System.Collections.Generic;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Page;

namespace Coralcode.Framework.Data.DynamicRepository
{
    /// <summary>
    /// 只提供查询的仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDynamicQueryRepository<TEntity> : IQueryRepository<TEntity, IDynamicSpecification<TEntity>> 
        where TEntity : class, IEntity,IDynamicRouter
    {
        List<TEntity> GetByIds(List<long> ids);
    }
}
