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
    public class StaticRepository<TEntity> : BaseStaticRepository<TEntity>
        where TEntity : class, IEntity
    {

        internal DbFactory DbFactory;
        internal ContextFactory ContextFactory;
        internal IEfUnitOfWork<TEntity> EfUnitOfWork;
        public StaticRepository(DbFactory dbFactory, ContextFactory contextFactory)
        {
            DbFactory = dbFactory;
            ContextFactory = contextFactory;
            EfUnitOfWork = new StaticUnitOfWork<TEntity>(contextFactory, dbFactory);
        }
        public override void Dispose()
        {
            base.Dispose();
            EfUnitOfWork = null;
            _entities = null;
            ContextFactory?.Dispose();
            ContextFactory = null;
        }

        /// <summary>
        ///获取所有的实体
        /// </summary>
        /// <returns>List of selected elements</returns>
        public override IQueryable<TEntity> GetAll()
        {
            return GetSet();
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
                item.Id = IdentityGenerator.NextId();

            GetSet().Add(item);
        }

        /// <summary>
        /// 删除实体 
        /// </summary>
        /// <param name="item"></param>
        public override void Remove(TEntity item)
        {
            GetSet().Remove(item);
        }

        /// <summary>
        /// 更改实体
        /// </summary>
        /// <param name="item"></param>
        public override void Modify(TEntity item)
        {
            EfUnitOfWork.SetModify(item, DbMode.Write | DbMode.Read);
        }

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override TEntity Get(long id)
        {
            return GetSet().FirstOrDefault(item => item.Id == id);
        }


        private DbSet<TEntity> _entities;
        protected virtual DbSet<TEntity> GetSet()
        {
            return _entities ?? (_entities =
                       EfUnitOfWork.CreateSet(DbFactory.GetStaticDbConfig(typeof(TEntity)), DbMode.Write | DbMode.Read));
        }

        protected IDbConnection GetDbConnection()
        {
           return ContextFactory.GetConnection(DbFactory.GetStaticDbInitContext(typeof(TEntity), DbMode.Write | DbMode.Read));
        }

        protected ISql GetSqlExetuator()
        {
            return ContextFactory.GetSqlExetuator(GetDbConnection());
        }
    }
}
