namespace Sharpduino.Handlers
{
    public interface IMessageHandler
    {   
        /// <summary>
        /// Find out if the handler can handle the next byte
        /// </summary>
        /// <param name="firstByte">The first byte of the message</param>
        /// <returns>True if the handle is able to handle the message</returns>
        bool CanHandle(byte firstByte);

        /// <summary>
        /// Handle the byte that came from the communication port
        /// </summary>
        /// <param name="messageByte">The byte that came from the port. It might be the first one, or a subsequent one</param>
        /// <returns>True if it should handle the next byte too</returns>
        bool Handle(byte messageByte);

        /// <summary>
        /// The START_MESSAGE byte for the current handler
        /// </summary>
        byte START_MESSAGE { get; }

        /// <summary>
        /// Reset the handler state so it can receive new messages
        /// </summary>
        void Reset();
    }
}