using System;
using System.Collections.Generic;
using Coralcode.Framework.Data.Core;

namespace Coralcode.Framework.Data.StaticRepository
{
    /// <summary>
    /// 提共增删改的仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IStaticCommandRepository<TEntity> : ICommandRepository<TEntity>, IDisposable where TEntity : class, IEntity
    {
    }
}
