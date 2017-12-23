using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;
using Coralcode.Framework.Data;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.DynamicRepository;
using Coralcode.Framework.Extensions;

namespace Coralcode.EntityFramework.ModelConfiguration
{
    public class DynamicEntityTypeConfig<TEntity> : StaticEntityTypeConfig<TEntity>
        where TEntity : class, IEntity, IDynamicRouter
    {
        protected override void Init()
        {
            Ignore(item => item.Coden);
            base.Init();
        }

    }
}
