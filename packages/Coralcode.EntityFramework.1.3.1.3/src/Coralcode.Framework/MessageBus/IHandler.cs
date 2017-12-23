namespace Coralcode.Framework.MessageBus
{
    /// <summary>
    /// Represents that the implemented classes are message handlers.
    /// </summary>
    /// <typeparam name="T">The type of the message to be handled.</typeparam>
    public interface IHandler<in T>
    {
        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        void Handle(T message);
    }
}