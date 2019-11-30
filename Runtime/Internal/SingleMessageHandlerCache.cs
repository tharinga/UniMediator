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
                return MissingReturnValue<T>(
                    $"No handler returning type {typeof(T)} is registered for {message}");
            }

            if (! (@delegate is Func<ISingleMessage<T>, T> func))
            {
                return MissingReturnValue<T>(
                    $"Handler returning type {typeof(T)} for {message} is null");
            }
            
            return func.Invoke(message);
        }

        // If there is no sane value to return, throw an Exception if running in 
        // Unity Editor. Return default(T) if running outside of Editor
        private T MissingReturnValue<T>(string errorMessage)
        {
        #if UNITY_EDITOR
            throw new UniMediatorException(errorMessage);
        #else
            Debug.LogError("UniMediator: " + errorMessage);
            return default(T);
        #endif
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