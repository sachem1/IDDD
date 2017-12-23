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
using Coralcode.Framework.Data.StaticRepository;

namespace Coralcode.EntityFramework.Repository.Static
{
    public class StaticQueryRepository<TEntity> : BaseStaticQueryRepository<TEntity>
        where TEntity : class, IEntity
    {
        private DbFactory _dbFactory;
        private ContextFactory _contextFactory;
        private IEfUnitOfWork<TEntity> _unitOfWork;
        public StaticQueryRepository(DbFactory dbFactory, ContextFactory contextFactory)
        {
            _dbFactory = dbFactory;
            _contextFactory = contextFactory;
            _unitOfWork = new StaticUnitOfWork<TEntity>(contextFactory, dbFactory);
        }

        public override void Dispose()
        {
            _unitOfWork?.Dispose();
            _contextFactory?.Dispose();
            _contextFactory = null;
        }

        /// <summary>
        ///获取所有的实体
        /// </summary>
        /// <returns>List of selected elements</returns>
        public override IQueryable<TEntity> GetAll()
        {
            //todo 数据权限
            return _unitOfWork.CreateSet(_dbFactory.GetStaticDbConfig(typeof(TEntity)), DbMode.Read);
        }
        protected IDbConnection GetDbConnection()
        {
            return _contextFactory.GetConnection(_dbFactory.GetStaticDbInitContext(typeof(TEntity), DbMode.Read));
        }

        protected ISql GetSqlExetuator()
        {
            return _contextFactory.GetSqlExetuator(GetDbConnection());
        }
    }
}
