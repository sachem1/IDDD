using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Coralcode.EntityFramework.Repository.Dynamic;
using Coralcode.EntityFramework.Repository.Static;
using Coralcode.Framework.Data;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.DynamicRepository;
using Coralcode.Framework.Data.Specification;
using Coralcode.Framework.Data.StaticRepository;
using EntityFramework.Extensions;

namespace Coralcode.EntityFramework.Extension
{
    /// <summary>
    /// 使用EntityFramwork.Extened组件做删除操作
    /// </summary>
    [Obsolete("为了兼容以前的程序，建议不要使用,改用Dapper做操作")]
    public static class RepositoryExtensions
    {
        public static int RemoveRange<TEntity>(this IDynamicCommandRepository<TEntity> repository, IDynamicSpecification<TEntity> specification)
            where TEntity : Entity, IDynamicRouter
        {
            var dynamicRepository = repository as DynamicRepository<TEntity>;
            if (dynamicRepository != null)
            {
                var dbset = dynamicRepository.EfUnitOfWork.CreateSet(dynamicRepository.DbFactory.GetDynamicDbConfig(typeof(TEntity), specification), DbMode.Read | DbMode.Write);
                return dbset.Where(specification.SatisfiedBy()).Delete();
            }
            var commandRepository = repository as DynamicCommandRepository<TEntity>;
            if (commandRepository != null)
            {
                var dbset = commandRepository.EfUnitOfWork.CreateSet(commandRepository.DbFactory.GetDynamicDbConfig(typeof(TEntity), specification), DbMode.Write);
                return dbset.Where(specification.SatisfiedBy()).Delete();
            }
            var separateRepository = repository as DynamicSeparateRepository<TEntity>;
            if (separateRepository != null)
            {
                var dbset = separateRepository.EfUnitOfWork.CreateSet(separateRepository.DbFactory.GetDynamicDbConfig(typeof(TEntity), specification), DbMode.Write);
                return dbset.Where(specification.SatisfiedBy()).Delete();
            }

            throw new InvalidOperationException("无法转换repository");
        }

        public static int RemoveRange<TEntity>(this IStaticCommandRepository<TEntity> repository, ISpecification<TEntity> specification)
            where TEntity : Entity
        {
            var separateRepository = repository as StaticSeparateRepository<TEntity>;
            if (separateRepository != null)
            {
                var dbset = separateRepository.EfUnitOfWork.CreateSet(separateRepository.DbFactory.GetStaticDbConfig((typeof(TEntity))), DbMode.Write);
                return dbset.Where(specification.SatisfiedBy()).Delete();
            }
            var staticRepository = repository as StaticRepository<TEntity>;
            if (staticRepository != null)
            {
                var dbset = staticRepository.EfUnitOfWork.CreateSet(staticRepository.DbFactory.GetStaticDbConfig(typeof(TEntity)), DbMode.Read | DbMode.Write);
                return dbset.Where(specification.SatisfiedBy()).Delete();
            }
            var commandRepository = repository as StaticCommandRepository<TEntity>;
            if (commandRepository != null)
            {
                var dbset = commandRepository.EfUnitOfWork.CreateSet(commandRepository.DbFactory.GetStaticDbConfig(typeof(TEntity)), DbMode.Write);
                return dbset.Where(specification.SatisfiedBy()).Delete();
            }

            throw new InvalidOperationException("无法转换repository");

        }

        public static int UpdateRange<TEntity>(this IDynamicCommandRepository<TEntity> repository, IDynamicSpecification<TEntity> specification, Expression<Func<TEntity, TEntity>> expression)
            where TEntity : Entity, IDynamicRouter
        {
            var dynamicRepository = repository as DynamicRepository<TEntity>;
            if (dynamicRepository != null)
            {
                var dbset = dynamicRepository.EfUnitOfWork.CreateSet(dynamicRepository.DbFactory.GetDynamicDbConfig(typeof(TEntity), specification), DbMode.Read | DbMode.Write);
                return dbset.Where(specification.SatisfiedBy()).Update(expression);
            }
            var commandRepository = repository as DynamicCommandRepository<TEntity>;
            if (commandRepository != null)
            {
                var dbset = commandRepository.EfUnitOfWork.CreateSet(commandRepository.DbFactory.GetDynamicDbConfig(typeof(TEntity), specification), DbMode.Write);
                return dbset.Where(specification.SatisfiedBy()).Update(expression);
            }
            var separateRepository = repository as DynamicSeparateRepository<TEntity>;
            if (separateRepository != null)
            {
                var dbset = separateRepository.EfUnitOfWork.CreateSet(separateRepository.DbFactory.GetDynamicDbConfig(typeof(TEntity), specification), DbMode.Write);
                return dbset.Where(specification.SatisfiedBy()).Update(expression);
            }

            throw new InvalidOperationException("无法转换repository");
        }

        public static int UpdateRange<TEntity>(this IStaticCommandRepository<TEntity> repository, ISpecification<TEntity> specification, Expression<Func<TEntity, TEntity>> expression)
            where TEntity : Entity
        {
            var separateRepository = repository as StaticSeparateRepository<TEntity>;
            if (separateRepository != null)
            {
                var dbset = separateRepository.EfUnitOfWork.CreateSet(separateRepository.DbFactory.GetStaticDbConfig((typeof(TEntity))), DbMode.Write);
                return dbset.Where(specification.SatisfiedBy()).Update(expression);
            }
            var staticRepository = repository as StaticRepository<TEntity>;
            if (staticRepository != null)
            {
                var dbset = staticRepository.EfUnitOfWork.CreateSet(staticRepository.DbFactory.GetStaticDbConfig(typeof(TEntity)), DbMode.Read | DbMode.Write);
                return dbset.Where(specification.SatisfiedBy()).Update(expression);
            }
            var commandRepository = repository as StaticCommandRepository<TEntity>;
            if (commandRepository != null)
            {
                var dbset = commandRepository.EfUnitOfWork.CreateSet(commandRepository.DbFactory.GetStaticDbConfig(typeof(TEntity)), DbMode.Write);
                return dbset.Where(specification.SatisfiedBy()).Update(expression);
            }

            throw new InvalidOperationException("无法转换repository");

        }
    }
}
