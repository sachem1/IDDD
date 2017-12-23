namespace Coralcode.Framework.MessageBus.Event
{
    public interface IEventHandler<in TEvent> : IHandler<TEvent>
        where TEvent : class, IEventData
    {
         
    }
}