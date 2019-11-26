using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniMediator.Internal;

namespace UniMediator 
{
    public sealed class MediatorImpl : MonoBehaviour, IMediator
    {
        private SingleMessageHandlerCache _singleMessageHandlers = new SingleMessageHandlerCache();
        
        private MulticastMessageHandlerCache _multicastMessageHandlers = new MulticastMessageHandlerCache();
        
        private ActiveObjectTracker _activeObjects = new ActiveObjectTracker();
        
        private static MediatorImpl _instance;

        private Scene _activeScene;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
               DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                DestroyImmediate(this);
                return;
            }
            
            _activeScene = SceneManager.GetActiveScene();
            ScanScene(_activeScene);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void Publish(IMulticastMessage message)
        {
            _multicastMessageHandlers.Invoke(message);
        }

        public T Send<T>(ISingleMessage<T> message)
        {
            return _singleMessageHandlers.Invoke(message);
        }
        
        public void AddMediatedObject(MonoBehaviour monoBehaviour)
        {
            ExtractHandlers(monoBehaviour);
        }

        private void ScanScene()
        {
            ScanScene(SceneManager.GetActiveScene());
        }
        
        private void ScanScene(Scene scene)
        {
            var monoBehaviours = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            for (int i = 0; i < monoBehaviours.Length; i++)
            {
                if (monoBehaviours[i].gameObject.scene == scene)
                {
                    ExtractHandlers(monoBehaviours[i]);
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene != _activeScene)
            {
                ScanScene(scene);
            }
        }

        private void ExtractHandlers(MonoBehaviour behaviour)
        {
            Type type = behaviour.GetType();

            if (type.ImplementsGenericInterface(typeof(IMulticastMessageHandler<>)) ||
                type.ImplementsGenericInterface(typeof(ISingleMessageHandler<,>)))
            {
                var methods = type.GetCachedMethods();
                
                for (int i = 0; i < methods.Length; i++)
                {
                    var parameters = methods[i].GetCachedParameters();

                    if (parameters.Length != 1) continue;
                        
                    var messageType = parameters[0].ParameterType;

                    if(typeof(IMulticastMessage).IsAssignableFrom(messageType))
                    {
                        CacheMulticastMessageHandler(messageType, behaviour, methods[i]);
                    }
                    
                    else if(messageType.ImplementsGenericInterface(typeof(ISingleMessage<>)))
                    {
                        CacheSingletMessageHandler(messageType, behaviour, methods[i]);
                    }
                }
            }
        }

        private void CacheMulticastMessageHandler(Type messageType, MonoBehaviour behavior, MethodInfo method)
        {
            var handler = _multicastMessageHandlers.CacheHandler(messageType, behavior, method);
            var remover = new MulticastMessageHandlerRemover(messageType, handler, _multicastMessageHandlers);
            AddLifeCycleMonitor(behavior.gameObject, remover);
        }

        private void CacheSingletMessageHandler(Type messageType, MonoBehaviour behavior, MethodInfo method)
        {
            var returnType = messageType.GetInterfaces()[0].GenericTypeArguments[0];
            _singleMessageHandlers.CacheHandler(messageType, returnType, behavior, method);
            var remover = new SingleMessageHandlerRemover(messageType, _singleMessageHandlers);
            AddLifeCycleMonitor(behavior.gameObject, remover);
        }

        private void AddLifeCycleMonitor(GameObject @object, IDelegateRemover remover)
        {
            if(!_activeObjects.Contains(@object))    
            {
                var monitor = @object.AddComponent<MediatorLifecycleMonitor>();
                monitor.ActiveObjects = _activeObjects; 
            }
            _activeObjects.AddActiveObject(@object, remover);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
        