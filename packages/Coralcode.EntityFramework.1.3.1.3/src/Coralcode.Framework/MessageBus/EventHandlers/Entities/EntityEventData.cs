using System;
using Coralcode.Framework.MessageBus.Event;

namespace Coralcode.Framework.MessageBus.EventHandlers.Entities
{
    /// <summary>
    /// Used to pass data for an event that is related to with an <see cref="IEntity"/> object.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    [Serializable]
    public class EntityEventData<TEntity> : EventData 
    {
        /// <summary>
        /// Related entity with this event.
        /// </summary>
        public TEntity Entity { get;  set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entity">Related entity with this event</param>
        public EntityEventData(TEntity entity)
        {
            Entity = entity;
        }
        
        public virtual object[] GetConstructorArgs()
        {
            return new object[] { Entity };
        }
    }
}