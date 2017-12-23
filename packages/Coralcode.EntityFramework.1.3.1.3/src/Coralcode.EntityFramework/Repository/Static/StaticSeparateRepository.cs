using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.EntityFramework.Extension;
using Coralcode.EntityFramework.UnitOfWork;
using Coralcode.Framework.Data;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.StaticRepository;

namespace Coralcode.EntityFramework.Repository.Static
{
    public class StaticSeparateRepository<TEntity> : StaticRepository<TEntity>
        where TEntity : class, IEntity
    {

        public StaticSeparateRepository(DbFactory dbFactory, ContextFactory contextFactory) :
            base(dbFactory, contextFactory)
        {
            EfUnitOfWork = new DynamicUnitOfWork<TEntity>(contextFactory, dbFactory);
        }
        public override void Dispose()
        {
            base.Dispose();
            EfUnitOfWork?.Dispose();
            EfUnitOfWork = null;
            ContextFactory?.Dispose();
            ContextFactory = null;
        }
        /// <summary>
        ///获取所有的实体
        /// </summary>
        /// <returns>List of selected elements</returns>
        public override IQueryable<TEntity> GetAll()
        {
            return EfUnitOfWork.CreateSet(DbFactory.GetStaticDbConfig(typeof(TEntity)), DbMode.Read);
        }

        /// <summary>
        /// 更改实体
        /// </summary>
        /// <param name="item"></param>
        public override void Modify(TEntity item)
        {
            EfUnitOfWork.SetModify(item, DbMode.Write);
        }


        protected override DbSet<TEntity> GetSet()
        {
            return EfUnitOfWork.CreateSet(DbFactory.GetStaticDbConfig(typeof(TEntity)), DbMode.Write);
        }

        protected IDbConnection GetDbConnection(DbMode model)
        {
            return ContextFactory.GetConnection(DbFactory.GetStaticDbInitContext(typeof(TEntity), model));
        }

        protected ISql GetSqlExetuator(DbMode model)
        {
            return ContextFactory.GetSqlExetuator(GetDbConnection(model));
        }
    }
}
