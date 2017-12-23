using System;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.MessageBus.Event;

namespace Coralcode.Framework.MessageBus.EventHandlers.Entities
{
    /// <summary>
    /// Used to trigger entity change events.
    /// </summary>
    public class EntityChangedEventHelper : IEntityChangedEventHelper
    {
        private readonly IEventBus _eventBus;

        private readonly IUnitOfWork _unitOfWorkManager;

        public EntityChangedEventHelper(IUnitOfWork unitOfWorkManager,IEventBus eventBus)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _eventBus = eventBus;
        }

        public void TriggerEntityCreatedEvent(object entity)
        {
            TriggerEntityChangeEvent(typeof(EntityCreatedEventData<>), entity);
        }

        public void TriggerEntityUpdatedEvent(object entity)
        {
            TriggerEntityChangeEvent(typeof(EntityModifyEventData<>), entity);
        }

        public void TriggerEntityDeletedEvent(object entity)
        {
            TriggerEntityChangeEvent(typeof(EntityRemoveEventData<>), entity);
        }

        private void TriggerEntityChangeEvent(Type genericEventType, object entity)
        {
            var entityType = entity.GetType();
            var eventType = genericEventType.MakeGenericType(entityType);

            if (_unitOfWorkManager == null)
            {
                _eventBus.Publish((IEventData)Activator.CreateInstance(eventType, new[] { entity }));
                return;
            }

            //_unitOfWorkManager.Current.Completed += (sender, args) => EventBus.Trigger(eventType, (IEventData)Activator.CreateInstance(eventType, new[] { entity }));
        }
    }
}