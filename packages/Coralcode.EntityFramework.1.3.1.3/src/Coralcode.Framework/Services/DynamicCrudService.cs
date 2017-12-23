using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.DynamicRepository;
using Coralcode.Framework.Data.Specification;
using Coralcode.Framework.MessageBus.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.Framework.Services
{
    public class DynamicCrudService
    {

        public abstract class DynamicCrudCoralService<TEntity, TModel, TSearch> : CrudCoralService<TEntity, TModel, TSearch, OrderBase, IDynamicSpecification<TEntity>>
           where TEntity : class, IEntity, IDynamicRouter, new()
           where TModel : class, IViewModel, new()
           where TSearch : SearchBase, IDynamicRouter
        {
            protected DynamicCrudCoralService(IDynamicRepository<TEntity> repository, IEventBus eventBus)
                : base(repository, eventBus)
            {
            }

            protected override IDynamicSpecification<TEntity> GetFilter(TSearch search)
            {
                return new SampleDynamicSpecificationn<TEntity>(new TrueSpecification<TEntity>(), search.Coden);
            }
        }

        public abstract class DynamicCrudCoralService<TEntity, TModel> : CrudCoralService<TEntity, TModel, SearchBase, OrderBase, IDynamicSpecification<TEntity>>
            where TEntity : class, IEntity, IDynamicRouter, new()
            where TModel : class, IViewModel, new()
        {
            protected DynamicCrudCoralService(IDynamicRepository<TEntity> repository, IEventBus eventBus)
                : base(repository, eventBus)
            {
            }
        }
    }
}
