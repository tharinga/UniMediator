using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniMediator.Internal
{
    internal sealed class MediatorLifecycleMonitor : MonoBehaviour
    {
        private void Start()
        {
            ;
        }

        public ActiveObjectTracker ActiveObjects { get; set; }
        private void OnDestroy()
        {
            ActiveObjects.RemoveActiveObject(gameObject);
        }
    }
}