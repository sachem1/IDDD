using System;
using System.Collections.Generic;
using Coralcode.Framework.Data.Core;

namespace Coralcode.Framework.Data.StaticRepository
{
    public class MemoryUnitOfWork:IUnitOfWork
    {
        private List<Action> actions = new List<Action>();

        public void Dispose()
        {
            actions.Clear();
        }

        internal void Register(Action action)
        {
            actions.Add(action);
        }

        /// <summary>
        /// 提交请求
        /// </summary>
        public void Commit()
        {
            actions.ForEach(item=>item());
        }

        /// <summary>
        /// 提交所有请求并处理乐观所的问题
        /// </summary>
        public void CommitAndRefreshChanges()
        {
            actions.ForEach(item => item());
        }

        /// <summary>
        /// 回滚所有的请求
        /// </summary>
        public void RollbackChanges()
        {
            actions.Clear();
        }
    }
}
