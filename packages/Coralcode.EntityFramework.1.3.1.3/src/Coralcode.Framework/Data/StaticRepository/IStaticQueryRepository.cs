using System;
using System.Linq;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.Specification;
using Coralcode.Framework.Page;

namespace Coralcode.Framework.Data.StaticRepository
{
    /// <summary>
    /// 只提供查询的仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IStaticQueryRepository<TEntity> : IQueryRepository<TEntity, ISpecification<TEntity>>
        where TEntity : class, IEntity
    {

    }
}
