namespace Sharpduino.Creators
{

    /// <summary>
    /// This is an interface for message creators. They get a message and return
    /// a byte array ready to be sent representing that message
    /// </summary>
    public interface IMessageCreator
    {
        /// <summary>
        /// Creates a byte array representing the given message
        /// </summary>
        /// <param name="message">The message we want to create</param>
        /// <returns>Byte array of all the bytes we need to send</returns>
        byte[] CreateMessage(object message);
    }

    public interface IMessageCreator<in T> : IMessageCreator
    {
        /// <summary>
        /// Creates a byte array representing the given message
        /// </summary>
        /// <param name="message">The message we want to create</param>
        /// <returns>Byte array of all the bytes we need to send</returns>
        byte[] CreateMessage(T message);
    }
}