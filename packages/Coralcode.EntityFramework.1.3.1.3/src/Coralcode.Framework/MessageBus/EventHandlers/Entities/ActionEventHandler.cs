using System;
using Coralcode.Framework.Data;
using Coralcode.Framework.MessageBus.Event;

namespace Coralcode.Framework.MessageBus.EventHandlers.Entities
{
    /// <summary>
    /// This event handler is an adapter to be able to use an action as <see cref="IEventHandler{TEvent}"/> implementation.
    /// </summary>
    /// <typeparam name="TEventData">Event type</typeparam>
    [Ignore]
    public class ActionEventHandler<TEventData> : IEventHandler<TEventData>
        where TEventData : class, IEventData
    {
        /// <summary>
        /// Action to handle the event.
        /// </summary>
        public Action<TEventData> Action { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="ActionEventHandler{TEventData}"/>.
        /// </summary>
        /// <param name="handler">Action to handle the event</param>
        public ActionEventHandler(Action<TEventData> handler)
        {
            Action = handler;
        }

        public void Handle(TEventData message)
        {
            Action(message);
        }
    }
}