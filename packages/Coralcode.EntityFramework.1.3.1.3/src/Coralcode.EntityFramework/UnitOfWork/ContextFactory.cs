using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Entity;
using Coralcode.EntityFramework.Extension;
using Coralcode.Framework.Aspect;
using Coralcode.Framework.Data;
using Coralcode.Framework.Exceptions;
using System.Collections.Generic;

namespace Coralcode.EntityFramework.UnitOfWork
{
    [Inject(RegisterType = typeof(ContextFactory), LifetimeManagerType = LifetimeManagerType.PerResolve)]
    public class ContextFactory : IDisposable
    {

        private ConcurrentDictionary<string, CoralDbContext> _contexts = new ConcurrentDictionary<string, CoralDbContext>();
        private List<IDbConnection> connnections = new List<IDbConnection>();

        private static Func<string, DbInitContext, CoralDbContext> _creator;
        private bool _isDispose;

        public ContextFactory()
        {
            if (_creator == null)
                _creator = (item, context) =>
                {
                    var dbContext = new CoralDbContext(context);
                    return dbContext;
                };
        }

        public static void SetContextCreator(Func<string, DbInitContext, CoralDbContext> creator)
        {
            _creator = creator;
        }

        /// <summary>
        /// 创建数据库上下文
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual CoralDbContext Create(DbInitContext context)
        {
            if (context == null)
                throw CoralException.ThrowException<DbErrorCode>(item => item.InvalideDbCoden, "上下文为空");
            if (context.Config == null)
                throw CoralException.ThrowException<DbErrorCode>(item => item.InvalideDbCoden, "上下文配置为空");
            if (string.IsNullOrEmpty(context.Config.NameOrConnectionString))
                throw CoralException.ThrowException<DbErrorCode>(item => item.InvalideDbCoden, "连接字符串为空");
            return _contexts.GetOrAdd(context.GetIdentity(), item => _creator(item, context));
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual IDbConnection GetConnection(DbInitContext context)
        {
            var connection = Database.DefaultConnectionFactory.CreateConnection(context.ConnectiongString);
            connnections.Add(connection);
            return connection;
        }


        /// <summary>
        /// 获取执行sql的接口
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public virtual ISql GetSqlExetuator(IDbConnection connection)
        {
            return new DapperSql(connection);
        }


        public void DisposeDbContext(string dbIdentity)
        {
            if (string.IsNullOrEmpty(dbIdentity))
                return;
            if (_contexts == null)
                return;
            CoralDbContext context;
            if (_contexts.TryRemove(dbIdentity, out context))
                context.Dispose();
        }

        public void Dispose()
        {
            if (_contexts != null)
            {
                foreach (var context in _contexts)
                {
                    context.Value?.Dispose();
                }
            }
            _contexts?.Clear();
            _contexts = null;
            connnections?.ForEach(item =>
            {
                item.Dispose();
            });
            connnections?.Clear();
            connnections = null;
        }

    }
}
