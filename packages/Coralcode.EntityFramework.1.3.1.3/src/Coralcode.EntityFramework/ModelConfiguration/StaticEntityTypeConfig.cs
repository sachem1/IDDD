using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;
using Coralcode.Framework.Data;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Extensions;

namespace Coralcode.EntityFramework.ModelConfiguration
{
    public class StaticEntityTypeConfig<TEntity> : EntityTypeConfiguration<TEntity>, IEntityConfiguration where TEntity : class, IEntity
    {
        public StaticEntityTypeConfig()
        {
            Init();
        }

        protected virtual void Init()
        {

            HasKey(item => item.Id);
            Property(item => item.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            typeof(TEntity).GetProperties().ForEach(item =>
            {
                if (!IgnoreAttribute.IsDefined(item))
                    return;
                var param = Expression.Parameter(typeof(TEntity), "item");
                var prorerty = Expression.Property(param, item.Name);
                //创建表达式 item=>item.property
                dynamic propertExpression = Expression.Lambda(prorerty, param);
                Ignore(propertExpression);
            });

            ToTable(typeof(TEntity).Name);
        }     
    }
}
