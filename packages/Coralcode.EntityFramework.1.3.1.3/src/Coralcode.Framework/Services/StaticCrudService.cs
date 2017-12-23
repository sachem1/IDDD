using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.Specification;
using Coralcode.Framework.Data.StaticRepository;
using Coralcode.Framework.MessageBus.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.Framework.Services
{
    public abstract class CrudCoralService<TEntity, TModel, TSearch> : CrudCoralService<TEntity, TModel, TSearch, OrderBase, ISpecification<TEntity>>
       where TEntity : class, IEntity, new()
       where TModel : class, IViewModel, new()
       where TSearch : SearchBase
    {
        protected CrudCoralService(IStaticRepository<TEntity> repository, IEventBus eventBus)
            : base(repository, eventBus)
        {
        }
        protected override ISpecification<TEntity> GetFilter(TSearch search)
        {
            return new TrueSpecification<TEntity>();
        }
    }

    public abstract class CrudCoralService<TEntity, TModel> : CrudCoralService<TEntity, TModel, SearchBase, OrderBase, ISpecification<TEntity>>
        where TEntity : class, IEntity, new()
        where TModel : class, IViewModel, new()
    {
        protected CrudCoralService(IStaticRepository<TEntity> repository, IEventBus eventBus)
            : base(repository, eventBus)
        {
        }
        protected override ISpecification<TEntity> GetFilter(SearchBase search)
        {
            return new TrueSpecification<TEntity>();
        }
    }
}
