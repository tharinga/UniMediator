namespace UniMediator
{
    /// <summary>
    /// Marker interface to represent a message that targets a single non-void handler method
    /// </summary>
    /// <typeparam name="TResponse">The return type of the handler method</typeparam>
    public interface ISingleMessage<out TResponse>
    {
    }
}