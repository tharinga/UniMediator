using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UniMediator.Internal
{
    internal sealed class MulticastMessageHandlerCache
    {
        private Dictionary<Type, MutableAction<IMulticastMessage>>  _handlers 
            = new Dictionary<Type, MutableAction<IMulticastMessage>>();

        public Action<IMulticastMessage> Add<TMessage>(Action<TMessage> handler)
        {
            var action = new Action<IMulticastMessage>(m => handler.Invoke((TMessage) m));
            var key = typeof(TMessage);
            if (!_handlers.ContainsKey(key))
            {
                _handlers[key] = new MutableAction<IMulticastMessage>(action);
            }
            else
            {
                _handlers[key] += action;
            }
            return action;
        }
        
        public void Remove(Type type, Action<IMulticastMessage> handler)
        {
            if (_handlers.ContainsKey(type))
            {
                _handlers[type] -= handler;
            }
        }

        public void Invoke(IMulticastMessage message)
        {
            var key = message.GetType();

            if (!_handlers.ContainsKey(key))
            {
                Debug.LogError("UniMediator: No handlers registered for: " + key);
                return;
            }
            _handlers[key].Invoke(message);
        }

        public Action<IMulticastMessage> CacheHandler(Type messageType, MonoBehaviour instance, MethodInfo method)
        {
            var handler = DelegateFactory.CreateMultiCastHandler(messageType, instance, method);

            var genericMethod = GetType()
                .GetCachedGenericMethod(nameof(Add), BindingFlags.Instance | BindingFlags.Public, messageType);
            return (Action<IMulticastMessage>) genericMethod.Invoke(this, new[] { handler });
        }
    }
}