using Coralcode.Framework.Data.Specification;

namespace Coralcode.Framework.Data.DynamicRepository
{
  
    public interface IDynamicSpecification<TEntity>:IDynamicRouter,ISpecification<TEntity>
        where TEntity : class 
    {
       
    }
}
