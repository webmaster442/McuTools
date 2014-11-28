using System;
using Sharpduino.Base;
using Sharpduino.Constants;
using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.Receive;

namespace Sharpduino.Handlers
{
    public class SysexStringMessageHandler : SysexMessageHandler<SysexStringMessage>
    {
        private HandlerState currentHandlerState;
        public const byte CommandByte = SysexCommands.STRING_DATA;
        protected new const string BaseExceptionMessage = "Error with the incoming byte. This is not a valid SysexStringMessage. ";

        private enum HandlerState
        {
            StartEnd,
            StringDataCommand,
            String
        }

        public SysexStringMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {
        }

        protected override void OnResetHandlerState()
        {
            currentHandlerState = HandlerState.StartEnd;
        }

        public override bool CanHandle(byte firstByte)
        {
            switch (currentHandlerState)
            {
                case HandlerState.StartEnd:
                    return firstByte == START_MESSAGE;
                case HandlerState.StringDataCommand:
                    return firstByte == CommandByte;
                case HandlerState.String:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override bool HandleByte(byte messageByte)
        {
            switch (currentHandlerState)
            {
                case HandlerState.StartEnd:
                    currentHandlerState = HandlerState.StringDataCommand;
                    return true;
                case HandlerState.StringDataCommand:
                    // MAX_DATA bytes AFTER the command byte
                    currentByteCount = 0;
                    currentHandlerState = HandlerState.String;
                    return true;
                case HandlerState.String:
                    if (messageByte == 0xF7)
                    {
                        // Get the string we have been building all along
                        message.Message = stringBuilder.ToString();
                        messageBroker.CreateEvent(message);
						Reset();                        
                        return false;
                    }
                    HandleChar(messageByte);
                    return true;
                default:
                    throw new MessageHandlerException("Unknown SysexMessage handler state");
            }
        }
    }
}
