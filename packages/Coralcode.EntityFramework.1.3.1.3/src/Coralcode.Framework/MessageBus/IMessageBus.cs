using System.Collections.Generic;

namespace Coralcode.Framework.MessageBus
{
    public interface IMessageBus
    {
        void Publish<TMessage>(TMessage message);

        void Publish<TMessage>(IEnumerable<TMessage> messages);
    }
}