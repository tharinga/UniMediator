using System.Collections.Generic;
using UnityEngine;

namespace UniMediator.Internal
{
    internal sealed class ActiveObjectTracker
    {
        private Dictionary<Object, LazySingleItemList<IDelegateRemover>> _activeObjects 
            = new Dictionary<Object, LazySingleItemList<IDelegateRemover>>();

        public bool Contains(Object key) => _activeObjects.ContainsKey(key);
        
        public void AddActiveObject(Object activeObject, IDelegateRemover remover)
        {
            // Note: removers is a Value Type
            _activeObjects.TryGetValue(activeObject, out var removers);
            removers.Add(remover);
            _activeObjects[activeObject] = removers;
        }
    
        public void RemoveActiveObject(Object @object)
        {
            _activeObjects.TryGetValue(@object, out var removers);
            
            for (int i = 0; i < removers.Count; i++)
            {
                removers[i].RemoveHandler();
            }
            
            _activeObjects.Remove(@object);
        }
    }
}