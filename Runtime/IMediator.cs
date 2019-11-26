using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniMediator
{
    public interface IMediator
    {
        /// <summary>
        /// Sends a message to a single handler with a non-void return type 
        /// </summary>
        /// <param name="message">The message object</param>
        /// <typeparam name="TResult">The return type of the handler</typeparam>
        /// <returns>The response of the handler</returns>
        TResult Send<TResult>(ISingleMessage<TResult> message);
        
        /// <summary>
        /// Sends a message to multiple handlers with a void return type
        /// </summary>
        /// <param name="message">The message object</param>
        void Publish(IMulticastMessage message);
        
        /// <summary>
        /// Registers a MonoBehavior with the Mediator. This is only necessary if
        /// an object is instantiated manually after the scene has loaded
        /// </summary>
        /// <param name="monoBehaviour"></param>
        void AddMediatedObject(MonoBehaviour monoBehaviour);
    }
}