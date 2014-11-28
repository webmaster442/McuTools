using System;
using System.Collections.Generic;
using System.Linq;
using Sharpduino.Logging;

namespace Sharpduino.Base
{
    public class MessageBroker : IMessageBroker
    {
        private readonly ILogger log;

        private readonly Dictionary<Type,List<IHandle>> handlers;

        public MessageBroker()
        {
            log = LogManager.CurrentLogger;
            log.Debug("Initializing messageBroker");
            this.handlers = new Dictionary<Type, List<IHandle>>();
        }

        public void Subscribe(object handler)
        {
            var interfaces = handler.GetType().GetInterfaces()
                .Where(x => typeof(IHandle).IsAssignableFrom(x) && x.IsGenericType);

            foreach (var iface in interfaces)
            {
                var type = iface.GetGenericArguments()[0];
            
                if (!handlers.ContainsKey(type))
                {
                    log.Debug("There was no handler for type {0}", type.ToString());
                    handlers[type] = new List<IHandle>();
                }

                handlers[type].Add((IHandle) handler);

                log.Info("Handler for message {0} subscribed", type.ToString());
            }
        }

        public void UnSubscribe(object handler)
        {
            var interfaces = handler.GetType().GetInterfaces()
                .Where(x => typeof(IHandle).IsAssignableFrom(x) && x.IsGenericType);

            foreach (var iface in interfaces)
            {
                var type = iface.GetGenericArguments()[0];
                
                if (!handlers.ContainsKey(type))
                {
                    log.Debug("There was no handler for type {0}", type.ToString());
                    continue;
                }

                handlers[type].Remove((IHandle) handler);
                
                log.Info("Handler for message {0} unsubscribed", type.ToString());
            }
        }

        public void CreateEvent<T>(T message)
        {
            if (handlers == null || !handlers.ContainsKey(typeof(T)) || handlers[typeof(T)].Count == 0)
            {
                log.Warn("There was no handler for message of type {0}", typeof(T).ToString());
                return;
            }

            foreach (var handler in handlers[typeof(T)])
            {
                try
                {
                    ((IHandle<T>)handler).Handle(message);
                }
                catch (InvalidCastException ex)
                {
                    log.Error("The handler was not of the right type. This should not have happened at all..." + ex.Message);
                    throw;
                }                
            }
        }
    }
}