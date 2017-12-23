using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.EntityFramework.Extension;
using Coralcode.Framework.Data;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.DynamicRepository;
using Coralcode.Framework.Extensions;

namespace Coralcode.EntityFramework.UnitOfWork
{
    public class DynamicUnitOfWork<TEntity> : IEfUnitOfWork<TEntity>
        where TEntity : class, IEntity
    {


        private List<CoralDbContext> _dbContexts = new List<CoralDbContext>();
        private ContextFactory _contextFactory;
        private DbFactory _dbFactory;
        private bool _isDisposed;

        public DynamicUnitOfWork(ContextFactory contextFactory, DbFactory dbFactory)
        {
            _contextFactory = contextFactory;
            _dbFactory = dbFactory;
        }
        public void Dispose()
        {
            if (_isDisposed)
                return;
            _dbContexts.ForEach(item =>
            {
                _contextFactory.DisposeDbContext(item?.CacheKey);
            });

            _dbContexts.Clear();
            _dbContexts = null;
            _isDisposed = true;
        }

        /// <summary>
        /// 提交请求
        /// </summary>
        public void Commit()
        {
            try
            {
                //这里如果没有调用过createset方法就会报错，如果没有调用认为没有任何改变直接跳出来
                _dbContexts.ForEach(item =>
                {
                    item.SaveChanges();
                });
            }
            catch (DbEntityValidationException ex)
            {
                throw ex.Format();
            }
        }

        /// <summary>
        /// 提交所有请求并处理乐观所的问题
        /// </summary>
        public void CommitAndRefreshChanges()
        {
            bool saveFailed;
            do
            {
                try
                {
                    _dbContexts.ForEach(item =>
                    {
                        item.SaveChanges();
                    });
                    saveFailed = false;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    ex.Entries.ToList()
                        .ForEach(entry => entry.OriginalValues.SetValues(entry.GetDatabaseValues()));
                }
                catch (DbEntityValidationException ex)
                {
                    throw ex.Format();
                }
            } while (saveFailed);
        }

        /// <summary>
        /// 回滚所有的请求
        /// </summary>
        public void RollbackChanges()
        {
            _dbContexts.ForEach(item =>
            {
                item.ChangeTracker.Entries()
                    .ToList()
                    .ForEach(entry =>
                    {
                        if (entry.State != EntityState.Unchanged)
                            entry.State = EntityState.Detached;
                    });
            });
        }

        /// <summary>
        /// 创建ef的dbSet 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public DbSet<TEntity> CreateSet(DbConfig config, DbMode mode)
        {
            var dbContext = _contextFactory.Create(_dbFactory.GetDbInitContext(typeof(TEntity), mode, config));
            if (!_dbContexts.Contains(dbContext)&& mode.HasFlag(DbMode.Write))
                _dbContexts.Add(dbContext);
            return dbContext.Set<TEntity>();
        }

        /// <summary>
        /// 设置更改
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="mode"></param>
        public void SetModify(TEntity entity, DbMode mode)
        {
            var dbContext = _contextFactory.Create(_dbFactory.GetDynamicDbInitContext(typeof(TEntity), mode, entity.Id));
            if (!_dbContexts.Contains(dbContext))
                _dbContexts.Add(dbContext);
            dbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
