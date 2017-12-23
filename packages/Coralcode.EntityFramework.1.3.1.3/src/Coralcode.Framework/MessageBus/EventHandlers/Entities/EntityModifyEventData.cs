using System;

namespace Coralcode.Framework.MessageBus.EventHandlers.Entities
{
    /// <summary>
    /// This type of event can be used to notify update of an Entity.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    [Serializable]
    public class EntityModifyEventData<TEntity> : EntityChangedEventData<TEntity>
    {
        public TEntity OldEntity { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="newEntity"></param>
        /// <param name="oldEntity"></param>
        public EntityModifyEventData(TEntity newEntity,TEntity oldEntity)
            : base(newEntity)
        {
            OldEntity = oldEntity;
        }
    }
}