using Sharpduino.Base;

namespace Sharpduino.Handlers
{
    public abstract class BaseMessageHandler<T> : BaseMessageHandler where T : new()
    {
        protected BaseMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {
            Reset();
        }

        /// <summary>
        /// The message that will be emited from the handler
        /// </summary>
        protected T message;

        public override void Reset()
        {
            message = new T();           
            OnResetHandlerState();
        }
    }

    public abstract class BaseMessageHandler : IMessageHandler
    {
        protected readonly IMessageBroker messageBroker;
        protected const string BaseExceptionMessage = "Error with the incoming byte.";

        /// <summary>
        /// The START_MESSAGE byte for the current handler
        /// </summary>
        public byte START_MESSAGE { get; protected set; }

        public abstract void Reset();


        protected BaseMessageHandler(IMessageBroker messageBroker)
        {
            this.messageBroker = messageBroker;
        }

        protected abstract void OnResetHandlerState();

        /// <summary>
        /// Find out if the handler can handle the next byte
        /// </summary>
        /// <param name="firstByte">The first byte of the message</param>
        /// <returns>True if the handle is able to handle the message</returns>
        public abstract bool CanHandle(byte firstByte);

        /// <summary>
        /// Handle the byte that came from the communication port
        /// </summary>
        /// <param name="messageByte">The byte that came from the port. It might be the first one, or a subsequent one</param>
        /// <returns>True if it should handle the next byte too</returns>
        public abstract bool Handle(byte messageByte);
    }
}
