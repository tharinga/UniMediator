using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniMediator.Internal
{
    // Because in this use case many delegates will be added/removed quickly
    // one after the other, immutable delegates aren't ideal for Unity
    
    internal struct MutableAction<T>
    {
        private LazySingleItemList<Action<T>> _handlers;

        public MutableAction(Action<T> action)
        {
            _handlers = new LazySingleItemList<Action<T>>();
            _handlers.Add(action);
        }
        public static MutableAction<T> operator + (MutableAction<T> action, Action<T> handler)
        {
            action._handlers.Add(handler);
            return action;
        }
        
        public static MutableAction<T> operator - (MutableAction<T> action, Action<T> handler)
        {
            action._handlers.FastRemove(handler);
            return action;
        }

        public void Invoke(T message)
        {
            for (int i = 0; i < _handlers.Count; i++)
            {
                _handlers[i]?.Invoke(message);
            }
        }
    }
}