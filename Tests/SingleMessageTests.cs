using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UniMediator;
using UnityEngine.TestTools;

namespace Tests
{
    public class SingleMessageTests
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
        public IEnumerator SingleMessageHandler_With_Reference_Return_Type_Replies_Correctly()
        {
            yield return null;
            var receiver = new GameObject("TestObject").AddComponent<HandlerWithReferenceReturnType>();
            var mediator = new GameObject(nameof(IMediator)).AddComponent<MediatorImpl>();
            receiver.Value = "pong";

            string result = mediator.Send(new MessageWithReferenceReturnType("ping"));

            Assert.That(result, Is.EqualTo("pingpong"));
        }
        
        [Test]
        public void SingleMessageHandler_With_Value_Return_Type_Replies_Correctly()
        {
            var receiver = new GameObject("TestObject").AddComponent<HandlerWithValueReturnType>();
            var mediator = new GameObject(nameof(IMediator)).AddComponent<MediatorImpl>();
            receiver.Value = 5;

            int result = mediator.Send(new MessageWithValueReturnType(10));

            Assert.That(result, Is.EqualTo(15));
        }
    }
    
    public class MessageWithReferenceReturnType : ISingleMessage<string>
    {
        public string Message;

        public MessageWithReferenceReturnType(string message)
        {
            Message = message;
        }
    }
    
    public class HandlerWithReferenceReturnType : MonoBehaviour, 
        ISingleMessageHandler<MessageWithReferenceReturnType, string>
    {
        public string Value;
        
        public string Handle(MessageWithReferenceReturnType message)
        {
            return message.Message + Value;
        }
    }
    
    public class MessageWithValueReturnType : ISingleMessage<int>
    {
        public int Message;

        public MessageWithValueReturnType(int message)
        {
            Message = message;
        }
    }
    
    public class HandlerWithValueReturnType : MonoBehaviour, 
        ISingleMessageHandler<MessageWithValueReturnType, int>
    {
        public int Value;
        
        public int Handle(MessageWithValueReturnType message)
        {
            return message.Message + Value;
        }
    }
 }
