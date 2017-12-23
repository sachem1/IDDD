using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.EntityFramework.Extension;
using Coralcode.EntityFramework.UnitOfWork;
using Coralcode.Framework.Data;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.DynamicRepository;

namespace Coralcode.EntityFramework.Repository.Dynamic
{
    public class DynamicQueryRepository<TEntity> : BaseDynamicQueryRepository<TEntity>
        where TEntity : class, IEntity, IDynamicRouter
    {

        protected DbFactory DbFactory;
        protected ContextFactory ContextFactory;
        protected IEfUnitOfWork<TEntity> EfUnitOfWork;
        public DynamicQueryRepository(DbFactory dbFactory, ContextFactory contextFactory) : base(dbFactory)
        {
            DbFactory = dbFactory;
            ContextFactory = contextFactory;
            EfUnitOfWork = new DynamicUnitOfWork<TEntity>(contextFactory, dbFactory);
        }

        public override void Dispose()
        {
            EfUnitOfWork?.Dispose();
            EfUnitOfWork = null;
            ContextFactory?.Dispose();
            ContextFactory = null;
        }

        /// <summary>
        /// 动态获取所有
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<TEntity> DynamicGetAll(IDynamicRouter router)
        {
            return EfUnitOfWork.CreateSet(DbFactory.GetDynamicDbConfig(typeof(TEntity), router), DbMode.Read);
        }

        protected IDbConnection GetDbConnection(short number)
        {
            return ContextFactory.GetConnection(DbFactory.GetDynamicDbInitContext(typeof(TEntity), DbMode.Read, number));
        }
        protected IDbConnection GetDbConnection(string coden)
        {
            return ContextFactory.GetConnection(DbFactory.GetDynamicDbInitContext(typeof(TEntity), DbMode.Read, coden));
        }

        protected ISql GetSqlExetuator(short number)
        {
            return ContextFactory.GetSqlExetuator(GetDbConnection(number));
        }
        protected ISql GetSqlExetuator(string coden)
        {
            return ContextFactory.GetSqlExetuator(GetDbConnection(coden));
        }
    }
}
