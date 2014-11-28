using System;
using Sharpduino.Base;
using Sharpduino.Constants;
using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.Receive;

namespace Sharpduino.Handlers
{
    public class CapabilityMessageHandler : SysexMessageHandler<CapabilityMessage>
    {
        private static readonly byte commandByte = SysexCommands.CAPABILITY_RESPONSE;
        private enum HandlerState
        {
            StartEnd,
            SysexCommand,
            PinMode,
            PinResolution
        }

        private HandlerState currentState;
        private PinModes currentMode;
        private byte currentPin;

        public CapabilityMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {}

        protected override void OnResetHandlerState()
        {
            currentState = HandlerState.StartEnd;
            currentPin = 0;
        }

        public override bool CanHandle(byte firstByte)
        {
            switch (currentState)
            {
                case HandlerState.StartEnd:
                    return firstByte == START_MESSAGE;
                case HandlerState.SysexCommand:
                    return firstByte == commandByte;
                case HandlerState.PinMode:                    
                case HandlerState.PinResolution:
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
                    currentState = HandlerState.PinMode;
                    return true;
                case HandlerState.PinMode:
                    // If the message has finished Reset and return that we won't handle other bytes
                    if (messageByte == MessageConstants.SYSEX_END)
                    {
                        Reset();
                        messageBroker.CreateEvent(new CapabilitiesFinishedMessage());
                        return false;
                    }

                    // We finished with a pin so create a capability message for this pin
                    // we don't change the currentState because the next byte could be
                    // a mode byte, or a finish message byte
                    if (messageByte == MessageConstants.FINISHED_PIN_CAPABILITIES)
                    {
                        message.PinNo = currentPin++;
                        messageBroker.CreateEvent(message);
                        message = new CapabilityMessage();
                        return true;
                    }

                    // Some assurance that we get an actual mode
                    if (messageByte > Enum.GetValues(typeof(PinModes)).Length)
                    {
                        Reset();
                        throw new MessageHandlerException(BaseExceptionMessage + "There is no such pin mode");
                    }
                    currentMode = (PinModes) messageByte;
                    currentState = HandlerState.PinResolution;
                    return true;
                case HandlerState.PinResolution:
                    message.Modes[currentMode] = messageByte;
                    currentState = HandlerState.PinMode;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
