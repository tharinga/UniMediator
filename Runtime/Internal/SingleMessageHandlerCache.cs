using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UniMediator.Internal
{
    internal sealed class SingleMessageHandlerCache
    {
        private Dictionary<Type, object> _handlers 
            = new Dictionary<Type, object>();
        
        public  void Add<TMessage, TReturn>(Func<TMessage, TReturn> handler)
        {
            _handlers[typeof(TMessage)] = new Func<ISingleMessage<TReturn>, TReturn>(
                message => handler.Invoke((TMessage) message));
        }
        
        public T Invoke<T>(ISingleMessage<T> message)
        {
            AotCodeGenerator<T>.RegisterGenericValueType();

            if (!_handlers.TryGetValue(message.GetType(), out var @delegate))
            {
                throw new UniMediatorException(
                    $"No handler returning type {typeof(T)} is registered for {message}");
            }

            if (! (@delegate is Func<ISingleMessage<T>, T> func))
            {
                throw new UniMediatorException(
                    $"Handler returning type {typeof(T)} for {message} is null");
            }
            
            return func.Invoke(message);
        }
        
        public void Remove(Type messageType)
        {
            if (!_handlers.ContainsKey(messageType))
            {
                return;
            }
            _handlers.Remove(messageType);
        }

        public void CacheHandler(
            Type messageType,
            Type returnType,
            MonoBehaviour instance, 
            MethodInfo method)
        {
            var handler = DelegateFactory.CreateSingleMessageHandler(messageType, returnType, instance, method);
             
            var genericMethod = GetType()
                .GetCachedGenericMethod(nameof(Add), BindingFlags.Instance | BindingFlags.Public, messageType, returnType);
                        
            genericMethod.Invoke(this, new[] { handler });
        }
    }
}