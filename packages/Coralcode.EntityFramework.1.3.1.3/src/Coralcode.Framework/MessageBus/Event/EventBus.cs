using System.Collections.Generic;
using Coralcode.Framework.Aspect;

namespace Coralcode.Framework.MessageBus.Event
{
    [Inject(RegisterType = typeof(IEventBus),LifetimeManagerType = LifetimeManagerType.ContainerControlled)]
    public class EventBus:IEventBus
    {
        private readonly IMessageDispatcher _iMessageDispatcher;
        public EventBus(IMessageDispatcher iMessageDispatcher)
        {
            _iMessageDispatcher = iMessageDispatcher;
        }

        public void Publish<TMessage>(TMessage message)
        {
            _iMessageDispatcher.DispatchMessage(message);

        }

        public void Publish<TMessage>(IEnumerable<TMessage> messages)
        {
            _iMessageDispatcher.DispatchMessage(messages);
        }
    }
}