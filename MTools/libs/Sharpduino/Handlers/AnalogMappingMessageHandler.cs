using System;
using Sharpduino.Base;
using Sharpduino.Constants;
using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.Receive;

namespace Sharpduino.Handlers
{
    public class AnalogMappingMessageHandler : SysexMessageHandler<AnalogMappingMessage>
    {
        private readonly byte commandByte = SysexCommands.ANALOG_MAPPING_RESPONSE;
        private HandlerState currentState;

        private enum HandlerState
        {
            StartEnd,
            SysexCommand,
            PinMapping
        }
        public AnalogMappingMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {}

        protected override void OnResetHandlerState()
        {
            currentState = HandlerState.StartEnd;            
        }

        public override bool CanHandle(byte firstByte)
        {
            switch (currentState)
            {
                case HandlerState.StartEnd:
                    return firstByte == START_MESSAGE;
                case HandlerState.SysexCommand:
                    return firstByte == commandByte;
                case HandlerState.PinMapping:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override bool HandleByte(byte messageByte)
        {
            switch (currentState)
            {
                case HandlerState.StartEnd:
                    currentState = HandlerState.SysexCommand;
                    return true;
                case HandlerState.SysexCommand:
                    currentState = HandlerState.PinMapping;
                    return true;
                case HandlerState.PinMapping:
                    if (messageByte == MessageConstants.SYSEX_END)
                    {
                        messageBroker.CreateEvent(message);
                        Reset();
                        return false;
                    }

                    if ( messageByte > 127 )
                        throw new MessageHandlerException(BaseExceptionMessage + "This is not a valid mapping");
                    message.PinMappings.Add(messageByte);
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
