using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coralcode.Framework.Aspect;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.Data;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Log;
using Coralcode.Framework.Reflection;

namespace Coralcode.Framework.MessageBus
{
    /// <summary>
    /// Represents the message dispatcher.
    /// </summary>
    [Inject(RegisterType = typeof(IMessageDispatcher), LifetimeManagerType = LifetimeManagerType.ContainerControlled)]
    public class MessageDispatcher : IMessageDispatcher
    {
        public MessageDispatcher()
        {
            MetaDataManager.Type.Find(item =>
            {
                if (!item.IsClass)
                    return false;
                if (IgnoreAttribute.IsDefined(item))
                    return false;
                var eventHandlerType =
                        item.GetInterfaces()
                        .Where(i => i.IsGenericType)
                        .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(IHandler<>));
                if (eventHandlerType == null)
                    return false;
                return true;
            }).ToList().ForEach(item =>
            {
                if(IgnoreAttribute.IsDefined(item))
                    return;
                RegisterType(this, item);
            });
        }


        #region Private Fields
        private readonly ConcurrentDictionary<Type, List<object>> _handlers = new ConcurrentDictionary<Type, List<object>>();
        #endregion

        #region Private Methods
        /// <summary>
        /// Registers the specified handler type to the message dispatcher.
        /// </summary>
        /// <param name="messageDispatcher">Message dispatcher instance.</param>
        /// <param name="handlerType">The type to be registered.</param>
        private static void RegisterType(IMessageDispatcher messageDispatcher, Type handlerType)
        {
            MethodInfo methodInfo = messageDispatcher.GetType().GetMethod("Register", new[] { typeof(Type) });

            var handlerIntfTypeQuery =handlerType.GetInterfaces()
                    .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IHandler<>));
            foreach (var handlerIntfType in handlerIntfTypeQuery)
            {
                UnityService.RegisterType(handlerIntfType, handlerType);
                var messageType = handlerIntfType.GetGenericArguments().First();
                MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(messageType);
                genericMethodInfo.Invoke(messageDispatcher, new object[] { handlerIntfType });
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Occurs when the message dispatcher is going to dispatch a message.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnDispatching(MessageDispatchEventArgs e)
        {
            var temp = this.Dispatching;
            if (temp != null)
                temp(this, e);
        }
        /// <summary>
        /// Occurs when the message dispatcher failed to dispatch a message.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnDispatchFailed(MessageDispatchEventArgs e)
        {
            var temp = this.DispatchFailed;
            if (temp != null)
                temp(this, e);
        }
        /// <summary>
        /// Occurs when the message dispatcher has finished dispatching the message.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnDispatched(MessageDispatchEventArgs e)
        {
            var temp = this.Dispatched;
            if (temp != null)
                temp(this, e);
        }
        #endregion

        #region IMessageDispatcher Members
        /// <summary>
        /// Clears the registration of the message handlers.
        /// </summary>
        public virtual void Clear()
        {
            _handlers.Clear();
        }

        /// <summary>
        /// Dispatches the message.
        /// </summary>
        /// <param name="message">The message to be dispatched.</param>
        public virtual void DispatchMessage<T>(T message)
        {
            var messageType = typeof(T);
            List<object> messageHandlers;
            if (!_handlers.TryGetValue(messageType, out messageHandlers)) return;
            messageHandlers.ForEach(messageHandler =>
            {
                HandlerMessage(messageHandler, message);
            });
        }

        /// <summary>
        /// batch dispatches the messages
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messages"></param>
        public virtual void DispatchMessage<T>(IEnumerable<T> messages)
        {
            var messageType = typeof(T);
            List<object> messageHandlers;
            if (!_handlers.TryGetValue(messageType, out messageHandlers)) return;
            messages.ForEach(message =>
            {
                messageHandlers.ForEach(messageHandler => HandlerMessage(messageHandler, message));
            });
        }

        private IHandler<T> GetHandler<T>(object handlerTypeOrInstance)
        {
            var instance = handlerTypeOrInstance as IHandler<T>;
            if (instance != null)
            {
                return instance;
            }
            return (IHandler<T>)UnityService.Resolve((Type)handlerTypeOrInstance);
        }

        private void HandlerMessage<T>(object messageHandlerInstanceOrType, T message)
        {
            var dynMessageHandler = GetHandler<T>(messageHandlerInstanceOrType);
            var evtArgs = new MessageDispatchEventArgs(message, messageHandlerInstanceOrType.GetType(), messageHandlerInstanceOrType);
            this.OnDispatching(evtArgs);
            try
            {
                dynMessageHandler.Handle(message);
                this.OnDispatched(evtArgs);
            }
            catch
            {
                this.OnDispatchFailed(evtArgs);
            }
        }


        /// <summary>
        /// Registers a messagehandlerType into message dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="handlerType">The handlerType to be registered.</param>
        public virtual void Register<T>(Type handlerType)
        {
            var keyType = typeof(T);

            _handlers.AddOrUpdate(keyType, key => new List<object> { handlerType },
               (key, value) =>
               {
                   if (!value.Contains(handlerType))
                       value.Add(handlerType);
                   return value;
               });


        }

        /// <summary>
        /// Unregisters a messagehandlerType from the message dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="handlerType">The handlerType to be registered.</param>
        public virtual void UnRegister<T>(Type handlerType)
        {
            var keyType = typeof(T);
            List<object> messageHandles;
            if (!_handlers.TryGetValue(keyType, out messageHandles))
                return;
            if (messageHandles.Contains(handlerType))
                messageHandles.Remove(handlerType);
        }


        /// <summary>
        /// Registers a messagehandlerinstance into message dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="handler">The handler to be registered.</param>
        public void Register<T>(IHandler<T> handler)
        {
            var keyType = typeof(T);

            _handlers.AddOrUpdate(keyType, key => new List<object> { handler },
               (key, value) =>
               {
                   if (!value.Contains(handler))
                       value.Add(handler);
                   return value;
               });
        }


        /// <summary>
        /// Unregisters a messagehandler from the message dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="handler">The handler to be registered.</param>
        public void UnRegister<T>(IHandler<T> handler)
        {
            var keyType = typeof(T);
            List<object> messageHandles;
            if (!_handlers.TryGetValue(keyType, out messageHandles))
                return;
            if (messageHandles.Contains(handler))
                messageHandles.Remove(handler);
        }

        /// <summary>
        /// Occurs when the message dispatcher is going to dispatch a message.
        /// </summary>
        public event EventHandler<MessageDispatchEventArgs> Dispatching;

        /// <summary>
        /// Occurs when the message dispatcher failed to dispatch a message.
        /// </summary>
        public event EventHandler<MessageDispatchEventArgs> DispatchFailed;

        /// <summary>
        /// Occurs when the message dispatcher has finished dispatching the message.
        /// </summary>
        public event EventHandler<MessageDispatchEventArgs> Dispatched;

        #endregion
    }
}