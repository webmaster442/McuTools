using System;
using Sharpduino.Base;
using Sharpduino.Constants;
using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.Receive;

namespace Sharpduino.Handlers
{
    public class I2CMessageHandler : SysexMessageHandler<I2CResponseMessage>
    {
        private enum HandlerState
        {
            StartEnd,
            SysexCommand,
            Address,
            Register,
            Data
        }

        private readonly byte commandByte = SysexCommands.I2C_REPLY;
        private HandlerState currentState;
        private bool firstByte;
        private byte byteCache;

        public I2CMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {}

        protected override void OnResetHandlerState()
        {
            currentState = HandlerState.StartEnd;
            firstByte = true;
            byteCache = 0;
        }

        public override bool CanHandle(byte firstByte)
        {
            switch (currentState)
            {
                case HandlerState.StartEnd:
                    return firstByte == START_MESSAGE;
                case HandlerState.SysexCommand:
                    return firstByte == commandByte;
                case HandlerState.Address:                    
                case HandlerState.Register:
                case HandlerState.Data:
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
                    currentState = HandlerState.Address;
                    return true;
                case HandlerState.Address:
                    if (messageByte > 127)
                    {
                        Reset();
                        throw new MessageHandlerException(BaseExceptionMessage + "This is not a valid address");
                    }
                    if (firstByte)
                    {
                        byteCache = messageByte;
                        firstByte = false;
                    }
                    else
                    {
                        message.SlaveAddress = BitHelper.BytesToInt(byteCache, messageByte);
                        currentState = HandlerState.Register;
                        firstByte = true;
                    }
                    return true;
                case HandlerState.Register:
                    if (messageByte > 127)
                    {
                        Reset();
                        throw new MessageHandlerException(BaseExceptionMessage + "This is not a valid register");
                    }
                    if (firstByte)
                    {
                        byteCache = messageByte;
                        firstByte = false;
                    }
                    else
                    {
                        message.Register = BitHelper.BytesToInt(byteCache, messageByte);
                        currentState = HandlerState.Data;
                        firstByte = true;
                    }
                    return true;
                case HandlerState.Data:
                    if (messageByte > 127 && messageByte != MessageConstants.SYSEX_END )
                    {
                        Reset();
                        throw new MessageHandlerException(BaseExceptionMessage + "These are not valid data");
                    }

                    if (firstByte)
                    {
                        if (message.Data.Count == 0 && messageByte == MessageConstants.SYSEX_END )
                        {
                            Reset();
                            throw new MessageHandlerException(BaseExceptionMessage + "There was no data incoming");
                        }

                        if (messageByte == MessageConstants.SYSEX_END)
                        {
                            messageBroker.CreateEvent(message);
                            Reset();
                            return false;
                        }
                        byteCache = messageByte;
                        firstByte = false;
                        return true;
                    }
                    else
                    {
                        if (messageByte == MessageConstants.SYSEX_END)
                        {
                            Reset();
                            throw new MessageHandlerException(BaseExceptionMessage + "The message should not end here");
                        }
                        message.Data.Add(BitHelper.BytesToInt(byteCache,messageByte));
                        firstByte = true;
                        return true;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
