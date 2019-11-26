using System;

namespace UniMediator.Internal
{
    internal sealed class SingleMessageHandlerRemover : IDelegateRemover
    {
        private Type _messageType;
        private SingleMessageHandlerCache _handlers;

        public SingleMessageHandlerRemover(
            Type messageType, 
            SingleMessageHandlerCache handlers)
        {
            _messageType = messageType;
            _handlers = handlers;
        }
        
        public void RemoveHandler()
        {
            _handlers.Remove(_messageType);
        }
    }
}