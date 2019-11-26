using NUnit.Framework;
using UnityEngine;
using UniMediator;

namespace Tests
{
    public class MulticastMessageTests
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
       
        [Test]
        public void Multicast_Message_Received_By_Multiple_Classes_With_Single_Handler()
        {
            var receiver1 = new GameObject("TestObject").AddComponent<MulticastReceiverOne>();
            var receiver2 = new GameObject("TestObject").AddComponent<MulticastReceiverOne>();
            var mediator = new GameObject(nameof(IMediator)).AddComponent<MediatorImpl>();
            
            mediator.Publish(new MulticastTestMessageOne(5));

            Assert.That(receiver1.Value + receiver2.Value, Is.EqualTo(10));
        }

        [Test] 
        public void Multicast_Message_Received_By_Single_Class_With_Multiple_Handlers()
        {
            var receiver = new GameObject("TestObject").AddComponent<MulticastReceiverTwo>();
            var mediator = new GameObject(nameof(IMediator)).AddComponent<MediatorImpl>();
            
            mediator.Publish(new MulticastTestMessageOne(5));
            mediator.Publish(new MulticastTestMessageTwo(10));

            Assert.That(receiver.Value1 + receiver.Value2, Is.EqualTo(15));
        }
    }
    
    public class MulticastTestMessageOne : IMulticastMessage
    {
        public int Value;

        public MulticastTestMessageOne(int value)
        {
            Value = value;
        }
    }
    
    public class MulticastReceiverOne : MonoBehaviour, 
        IMulticastMessageHandler<MulticastTestMessageOne>
    {
        public int Value = 0;
        
        public void Handle(MulticastTestMessageOne message)
        {
            Value = message.Value;
        }
    }
    
    
    public class MulticastTestMessageTwo : IMulticastMessage
    {
        public int Value;

        public MulticastTestMessageTwo(int value)
        {
            Value = value;
        }
    }
    
    public class MulticastReceiverTwo : MonoBehaviour, 
        IMulticastMessageHandler<MulticastTestMessageOne>, 
        IMulticastMessageHandler<MulticastTestMessageTwo>
    {
        public int Value1 = 0;
        public int Value2 = 0;
        
        public void Handle(MulticastTestMessageOne message)
        {
            Value1 = message.Value;
        }

        public void Handle(MulticastTestMessageTwo message)
        {
            Value2 = message.Value;
        }
    }
}
