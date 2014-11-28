using System.Text;
using Sharpduino.Base;
using Sharpduino.Constants;
using Sharpduino.Exceptions;

namespace Sharpduino.Handlers
{
    public abstract class SysexMessageHandler<T> : BaseMessageHandler<T> where T : new()
    {
        protected StringBuilder stringBuilder;
        protected int  currentByteCount;
        protected byte cacheChar;
        
        protected SysexMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {
            START_MESSAGE = MessageConstants.SYSEX_START;
        }

        public override void Reset()
        {
            base.Reset();
            stringBuilder = new StringBuilder();
            currentByteCount = 0;
            cacheChar = 255;
        }

        protected abstract bool HandleByte(byte messageByte);

        public override bool Handle(byte messageByte)
        {
            if (!CanHandle(messageByte))
            {
                // Reset the state of the handler
                Reset();
                throw new MessageHandlerException(BaseExceptionMessage);
            }

            if (++currentByteCount > MessageConstants.MAX_DATA_BYTES)
            {
                // Reset the state of the handler
                Reset();
                throw new MessageHandlerException(BaseExceptionMessage + "Max message length was exceeded.");
            }

            return HandleByte(messageByte);
        }

        protected void HandleChar(byte charByte)
        {
            if (cacheChar != 255)
            {
                stringBuilder.Append((char)BitHelper.BytesToInt(cacheChar, charByte));
                cacheChar = 255;
            }
            else
                cacheChar = charByte;
        }
    }
}