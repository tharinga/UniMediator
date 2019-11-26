namespace UniMediator
{
    /// <summary>
    /// Defines a Handler for a Multicast Message of type TMessage
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMulticastMessageHandler<in TMessage> 
        where TMessage : IMulticastMessage
    {
        /// <summary>
        /// Handle a Multicast Message
        /// </summary>
        /// <param name="message">The message to handle</param>
        void Handle(TMessage message);
    }
}