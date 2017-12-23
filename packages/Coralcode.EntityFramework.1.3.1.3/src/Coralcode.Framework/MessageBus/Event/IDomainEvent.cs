namespace Coralcode.Framework.MessageBus.Event
{
    public interface IDomainEvent:IEventData
    {
        IMessage Source { get; set; }
        /// <summary>
        /// Gets or sets the version of the domain event.
        /// </summary>
        long Version { get; set; }
        /// <summary>
        /// Gets or sets the branch on which the current domain event exists.
        /// </summary>
        long Branch { get; set; }
    }
}