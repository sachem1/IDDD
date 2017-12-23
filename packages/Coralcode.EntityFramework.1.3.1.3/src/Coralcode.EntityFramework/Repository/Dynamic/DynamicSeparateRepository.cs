using System;
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
using EntityFramework.Batch;

namespace Coralcode.EntityFramework.Repository.Dynamic
{
    public class DynamicSeparateRepository<TEntity> : BaseDynamicRepository<TEntity>
        where TEntity : class, IEntity, IDynamicRouter
    {
        internal DbFactory DbFactory;
        internal ContextFactory ContextFactory;
        internal IEfUnitOfWork<TEntity> EfUnitOfWork;
        public DynamicSeparateRepository(DbFactory dbFactory, ContextFactory contextFactory) :
            base(dbFactory)
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

        /// <summary>
        ///工作单元
        /// </summary>
        public override IUnitOfWork UnitOfWork => EfUnitOfWork;

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="item"></param>
        public override void Add(TEntity item)
        {
            if (item.Id == 0)
                item.Id = IdentityGenerator.NextId(DbFactory.GetDynamicDbConfig(typeof(TEntity), item).Number);

            EfUnitOfWork.CreateSet(DbFactory.GetDynamicDbConfig(typeof(TEntity), item), DbMode.Write).Add(item);
        }

        /// <summary>
        /// 删除实体 
        /// </summary>
        /// <param name="item"></param>
        public override void Remove(TEntity item)
        {
            EfUnitOfWork.CreateSet(DbFactory.GetDynamicDbConfig(typeof(TEntity), item), DbMode.Write).Remove(item);
        }

        /// <summary>
        /// 更改实体
        /// </summary>
        /// <param name="item"></param>
        public override void Modify(TEntity item)
        {
            EfUnitOfWork.SetModify(item, DbMode.Write);
        }

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override TEntity Get(long id)
        {
            return EfUnitOfWork.CreateSet(DbFactory.GetDynamicDbConfig(typeof(TEntity), id), DbMode.Write).FirstOrDefault(item => item.Id == id);
        }
        protected IDbConnection GetDbConnection(short number, DbMode model)
        {
            return ContextFactory.GetConnection(DbFactory.GetDynamicDbInitContext(typeof(TEntity), model, number));
        }
        protected IDbConnection GetDbConnection(string coden, DbMode model)
        {
            return ContextFactory.GetConnection(DbFactory.GetDynamicDbInitContext(typeof(TEntity), model, coden));
        }

        protected ISql GetSqlExetuator(short number, DbMode model)
        {
            return ContextFactory.GetSqlExetuator(GetDbConnection(number, model));
        }
        protected ISql GetSqlExetuator(string coden, DbMode model)
        {
            return ContextFactory.GetSqlExetuator(GetDbConnection(coden, model));
        }
    }
}
