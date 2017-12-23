using System;

namespace Coralcode.Framework.Data.Core
{
    public interface IUnitOfWork : IDisposable
    { 
        /// <summary>
        /// 提交请求
        /// </summary>
        void Commit();

        /// <summary>
        /// 提交所有请求并处理乐观所的问题
        /// </summary>
        void CommitAndRefreshChanges();

        /// <summary>
        /// 回滚所有的请求
        /// </summary>
        void RollbackChanges();
    }
}
