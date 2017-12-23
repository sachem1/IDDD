using Coralcode.Framework.Data.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.Framework.Data.DynamicRepository
{
    public class SampleDynamicSpecificationn<TEntity> : IDynamicSpecification<TEntity>
          where TEntity : class
    {

        private ISpecification<TEntity> _specification;
        public SampleDynamicSpecificationn(ISpecification<TEntity> specification, string coden)
        {
            _specification = specification;
            Coden = coden;
        }
        public string Coden { get; }

        public Expression<Func<TEntity, bool>> SatisfiedBy()
        {
            throw new NotImplementedException();
        }
    }
}
