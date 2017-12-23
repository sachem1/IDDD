using System;

namespace Coralcode.Framework.MessageBus.EventHandlers.Entities
{
    /// <summary>
    /// This type of event can be used to notify deletion of an Entity.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    [Serializable]
    public class EntityRemoveEventData<TEntity> : EntityChangedEventData<TEntity>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entity">The entity which is deleted</param>
        public EntityRemoveEventData(TEntity entity)
            : base(entity)
        {

        }
    }
}