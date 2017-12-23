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
using Coralcode.Framework.Extensions;

namespace Coralcode.EntityFramework.Repository.Static
{
    public class StaticCommandRepository<TEntity> : IStaticCommandRepository<TEntity>
        where TEntity : class, IEntity
    {
        internal DbFactory DbFactory;
        internal ContextFactory ContextFactory;
        internal IEfUnitOfWork<TEntity> EfUnitOfWork;
        public StaticCommandRepository(DbFactory dbFactory, ContextFactory contextFactory)
        {
            DbFactory = dbFactory;
            ContextFactory = contextFactory;
            EfUnitOfWork = new StaticUnitOfWork<TEntity>(contextFactory, dbFactory);
        }
        public void Dispose()
        {
            EfUnitOfWork?.Dispose();
            EfUnitOfWork = null;
            _entities = null;
            ContextFactory?.Dispose();
            ContextFactory = null;
        }

        /// <summary>
        ///工作单元
        /// </summary>
        public IUnitOfWork UnitOfWork => EfUnitOfWork;

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="item"></param>
        public void Add(TEntity item)
        {
            if (item.Id == 0)
                item.Id = IdentityGenerator.NextId();
            GetSet().Add(item);
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<TEntity> items)
        {
            items.ForEach(Add);
        }

        /// <summary>
        /// 删除实体 
        /// </summary>
        /// <param name="item"></param>
        public void Remove(TEntity item)
        {
            GetSet().Remove(item);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="items"></param>
        public void RemoveRange(IEnumerable<TEntity> items)
        {
            GetSet().RemoveRange(items);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="ids"></param>
        public void RemoveRange(IEnumerable<long> ids)
        {
            ids.ForEach(Remove);
        }

        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <param name="id">主键</param>
        public void Remove(long id)
        {
            Remove(Get(id));
        }

        /// <summary>
        /// 更改实体
        /// </summary>
        /// <param name="item"></param>
        public void Modify(TEntity item)
        {
            EfUnitOfWork.SetModify(item,DbMode.Write);
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="items"></param>
        public void ModifyRange(IEnumerable<TEntity> items)
        {
            items.ForEach(Modify);
        }

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity Get(long id)
        {
            return GetSet().FirstOrDefault(item => item.Id == id);
        }

        private DbSet<TEntity> _entities;
        protected DbSet<TEntity> GetSet()
        {
            return _entities ?? (_entities =
                       EfUnitOfWork.CreateSet(DbFactory.GetStaticDbConfig(typeof(TEntity)), DbMode.Write));
        }
        protected IDbConnection GetDbConnection()
        {
            return ContextFactory.GetConnection(DbFactory.GetStaticDbInitContext(typeof(TEntity), DbMode.Write));
        }

        protected ISql GetSqlExetuator()
        {
            return ContextFactory.GetSqlExetuator(GetDbConnection());
        }
    }
}
