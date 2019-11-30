using System;
using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;
using static System.Linq.Enumerable;
using UnityEngine;
using UnityEngine.TestTools;
using UniMediator;
using UniMediator.Internal;
using Object = UnityEngine.Object;

namespace Tests
{
    public class LifeCycleTests
    {
        [SetUp]
        public void SetUp()
        {
            TestUtilities.SetUp();
        }

        [TearDown]
        public void TearDown()
        {
            TestUtilities.TearDown();
        }
        
        [UnityTest]
        public IEnumerator Multicast_Handlers_Unsubscribe_After_Destroy()
        {
            int counter = 0;

            var message = new PingBackMessage(() => counter += 1);
            
            var listeners = new PingBackHandler[10];

            // create 10 handlers
            foreach (var n in Range(0,10))
            {
                listeners[n] = new GameObject(n.ToString()).AddComponent<PingBackHandler>();
            }
            
            var mediator = new GameObject(nameof(IMediator)).AddComponent<MediatorImpl>();
            
            // counter incremented 10 times
            mediator.Publish(message);
            
            // delete 5 handlers
            foreach (var n in Range(5,5))
            {
                Object.Destroy(listeners[n].gameObject);
            }
            
            yield return null;
            
            // counter incremented 5 times
            mediator.Publish(message);
            
            Assert.That(counter, Is.EqualTo(15));
        }
        
        [UnityTest]
        public IEnumerator Single_Message_Handler_Unsubscribes_On_Destroy()
        {
            var receiver = new GameObject("TestObject").AddComponent<HandlerWithValueReturnType>();
            var mediator = new GameObject(nameof(IMediator)).AddComponent<MediatorImpl>();

            mediator.Send(new MessageWithValueReturnType(0));

            Object.Destroy(receiver.gameObject);
            
            yield return null;
            
        #if UNITY_EDITOR
            Assert.Throws<UniMediatorException>(() => mediator.Send(new MessageWithValueReturnType(0)));
        #else
            mediator.Send(new MessageWithValueReturnType(0));
            LogAssert.Expect(LogType.Error, "UniMediator: No handler returning type System.Int32 is registered for Tests.MessageWithValueReturnType");
        #endif
            
        }
    }
    
    public class PingBackMessage : IMulticastMessage
    {
        public Action PingBackAction;

        public PingBackMessage(Action action)
        {
            PingBackAction = action;
        }
    }
    
    public class PingBackHandler : MonoBehaviour, 
        IMulticastMessageHandler<PingBackMessage>
    {
        public void Handle(PingBackMessage message)
        {
            message.PingBackAction();
        }
    }
}