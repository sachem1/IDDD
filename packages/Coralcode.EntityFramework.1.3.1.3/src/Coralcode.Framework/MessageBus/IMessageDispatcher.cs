using System;
using System.Collections.Generic;

namespace Coralcode.Framework.MessageBus
{
    /// <summary>
    /// Represents the message dispatcher.
    /// </summary>
    public interface IMessageDispatcher
    {
        /// <summary>
        /// Clears the registration of the message handlers.
        /// </summary>
        void Clear();
        /// <summary>
        /// Dispatches the message.
        /// </summary>
        /// <param name="message">The message to be dispatched.</param>
        void DispatchMessage<T>(T message);

        /// <summary>
        /// 分发多个消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messages"></param>
        void DispatchMessage<T>(IEnumerable<T> messages);

        /// <summary>
        /// Registers a message handler into message dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="handlerType">The handler to be registered.</param>
        void Register<T>(Type handlerType);
        /// <summary>
        /// Unregisters a message handler from the message dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="handlerType">The handler to be registered.</param>
        void UnRegister<T>(Type handlerType);

        /// <summary>
        /// Registers a message handler into message dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="handler">The handler to be registered.</param>
        void Register<T>(IHandler<T> handler);
        /// <summary>
        /// Unregisters a message handler from the message dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="handler">The handler to be registered.</param>
        void UnRegister<T>(IHandler<T> handler);


        /// <summary>
        /// Occurs when the message dispatcher is going to dispatch a message.
        /// </summary>
        event EventHandler<MessageDispatchEventArgs> Dispatching;
        /// <summary>
        /// Occurs when the message dispatcher failed to dispatch a message.
        /// </summary>
        event EventHandler<MessageDispatchEventArgs> DispatchFailed;
        /// <summary>
        /// Occurs when the message dispatcher has finished dispatching the message.
        /// </summary>
        event EventHandler<MessageDispatchEventArgs> Dispatched;
    } 
}