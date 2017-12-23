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
using Coralcode.Framework.Extensions;

namespace Coralcode.EntityFramework.Repository.Dynamic
{
    public class DynamicCommandRepository<TEntity> : IDynamicCommandRepository<TEntity>
        where TEntity : class, IEntity, IDynamicRouter
    {
        internal DbFactory DbFactory;
        internal ContextFactory ContextFactory;
        internal IEfUnitOfWork<TEntity> EfUnitOfWork;
        public DynamicCommandRepository(DbFactory dbFactory, ContextFactory contextFactory)
        {
            DbFactory = dbFactory;
            ContextFactory = contextFactory;
            EfUnitOfWork = new DynamicUnitOfWork<TEntity>(contextFactory, dbFactory);
        }
        public void Dispose()
        {
            EfUnitOfWork?.Dispose();
            EfUnitOfWork = null;
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
                item.Id = IdentityGenerator.NextId(DbFactory.GetDynamicDbConfig(typeof(TEntity), item).Number);

            EfUnitOfWork.CreateSet(DbFactory.GetDynamicDbConfig(typeof(TEntity), item), DbMode.Write).Add(item);
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
            EfUnitOfWork.CreateSet(DbFactory.GetDynamicDbConfig(typeof(TEntity), item), DbMode.Write).Remove(item);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="items"></param>
        public void RemoveRange(IEnumerable<TEntity> items)
        {
            items.ForEach(Remove);
        }
        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <param name="id">主键</param>
        public void Remove(long id)
        {
            var item = Get(id);
            EfUnitOfWork.CreateSet(DbFactory.GetDynamicDbConfig(typeof(TEntity), item), DbMode.Write).Remove(item);
        }


        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="ids"></param>
        public void RemoveRange(IEnumerable<long> ids)
        {
            ids.ForEach(Remove); ;
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
            return EfUnitOfWork.CreateSet(DbFactory.GetDynamicDbConfig(typeof(TEntity), id), DbMode.Write).FirstOrDefault(item => item.Id == id);
        }


        protected IDbConnection GetDbConnection(short number)
        {
            return ContextFactory.GetConnection(DbFactory.GetDynamicDbInitContext(typeof(TEntity), DbMode.Write, number));
        }
        protected IDbConnection GetDbConnection(string coden)
        {
            return ContextFactory.GetConnection(DbFactory.GetDynamicDbInitContext(typeof(TEntity), DbMode.Write, coden));
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
