using System;

namespace Coralcode.Framework.MessageBus.EventHandlers.Entities
{
    /// <summary>
    /// Used to pass data for an event that is related to with a changed <see cref="IEntity"/> object.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    [Serializable]
    public class EntityQueryingEventData<TEntity> : EntityEventData<TEntity>
    {


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entity">Changed entity in this event</param>
        public EntityQueryingEventData(TEntity entity)
            : base(entity)
        {

        }
    }
}