using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.CompilerServices;
using Coralcode.EntityFramework.Extension;
using Coralcode.Framework.Aspect;
using Coralcode.Framework.Data;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Log;
using Coralcode.Framework.Modules;
using EntityFramework.Extensions;

namespace Coralcode.EntityFramework.UnitOfWork
{
    public class StaticUnitOfWork<TEntity> : IEfUnitOfWork<TEntity>
        where TEntity : class, IEntity
    {
        private IModuleManager _moduleManager;
        private readonly ContextFactory _contextFactory;
        private readonly DbFactory _dbFactory;
        private CoralDbContext _dbContext;
        private bool _isDisposed;


        public StaticUnitOfWork(ContextFactory contextFactory, DbFactory dbFactory)
        {
            _contextFactory = contextFactory;
            _dbFactory = dbFactory;
        }

        #region IQueryableUnitOfWork Members
        public DbSet<TEntity> CreateSet(DbConfig config, DbMode mode)
        {
            if (_dbContext == null)
                _dbContext = _contextFactory.Create(_dbFactory.GetDbInitContext(typeof(TEntity), mode, config));
            return _dbContext.Set<TEntity>();
        }

        public void SetModify(TEntity entity, DbMode mode)
        {
            if (_dbContext == null)
                return;

            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Commit()
        {
            try
            {
                _dbContext?.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                throw ex.Format();
            }
        }

        public void CommitAndRefreshChanges()
        {
            bool saveFailed;
            do
            {
                try
                {
                    _dbContext.SaveChanges();
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

        public void RollbackChanges()
        {
            _dbContext.ChangeTracker.Entries()
                         .ToList()
                         .ForEach(entry =>
                         {
                             if (entry.State != EntityState.Unchanged)
                                 entry.State = EntityState.Detached;
                         });
        }
        #endregion

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            if (_isDisposed)
                return;
            _dbContext?.Dispose();
            _contextFactory.DisposeDbContext(_dbContext?.CacheKey);
            _dbContext = null;
            _isDisposed = true;
        }
    }
}
