using Coralcode.Framework.Data.Core;

namespace Coralcode.Framework.Data.DynamicRepository
{
    /// <summary>
    /// 提共增删改的仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDynamicCommandRepository<TEntity> : ICommandRepository<TEntity> where TEntity : class, IEntity, IDynamicRouter
    {

    }
}
