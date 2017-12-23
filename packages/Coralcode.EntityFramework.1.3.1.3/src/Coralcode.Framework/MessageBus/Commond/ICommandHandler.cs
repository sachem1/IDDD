namespace Coralcode.Framework.MessageBus.Commond
{
    public interface ICommandHandler<in TEvent> : IHandler<TEvent>
        where TEvent : class, ICommandBus
    {
         
    }
}