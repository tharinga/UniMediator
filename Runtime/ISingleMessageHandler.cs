namespace UniMediator
{
    /// <summary>
    /// Defines a Handler for a message of type <see cref="ISingleMessage{T}"/> 
    /// </summary>
    /// <typeparam name="TMessage">The Type of the message parameter for this handler</typeparam>
    /// <typeparam name="TResponse">The return Type of the handler method</typeparam>
    public interface ISingleMessageHandler<in TMessage, out TResponse> 
        where TMessage : ISingleMessage<TResponse> 
    {
        /// <summary>
        /// Handle the message
        /// </summary>
        /// <param name="message">The message being handled</param>
        /// <returns>Response from the handler</returns>
        TResponse Handle(TMessage message);
    }
}