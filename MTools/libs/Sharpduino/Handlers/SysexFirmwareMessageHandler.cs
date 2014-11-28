using System;
using Sharpduino.Base;
using Sharpduino.Constants;
using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.Receive;

namespace Sharpduino.Handlers
{
    /// <summary>
    /// Class that handles the receipt of the firmware sysex message
    /// The firmware name to be reported should be exactly the same as the name of the Arduino file, minus the .pde. So for Standard_Firmata.pde, the firmware name is: Standard_Firmata.
    /// Receive Firmware Name and Version (after query)
    /// 0  START_MESSAGE (0xF0)
    /// 1  queryFirmware (0x79)
    /// 2  major version (0-127)
    /// 3  minor version (0-127)
    /// 4  first 7-bits of firmware name
    /// 5  second 7-bits of firmware name
    /// x  ...for as many bytes as it needs)
    /// 6  END_SYSEX (0xF7)
    /// </summary>
    public class SysexFirmwareMessageHandler : SysexMessageHandler<SysexFirmwareMessage>
    {
        private const byte CommandByte = Constants.SysexCommands.QUERY_FIRMWARE;

        protected new const string BaseExceptionMessage = "Error with the incoming byte. This is not a valid SysexFirmwareMessage. ";

        private enum HandlerState
        {
            StartEnd,
            StartSysex,
            QueryFirmware,
            MajorVersion,
            MinorVersion,
            FirmwareName
        }

        private HandlerState currentHandlerState;

        public SysexFirmwareMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {}

        protected override void OnResetHandlerState()
        {
            currentHandlerState = HandlerState.StartEnd;
        }

        /// <summary>
        /// Find out if the handler can handle the next byte
        /// </summary>
        /// <param name="firstByte">The first byte of the message</param>
        /// <returns>True if the handle is able to handle the message</returns>
        public override bool CanHandle(byte firstByte)
        {
            switch (currentHandlerState)
            {
                case HandlerState.StartSysex:
                    return firstByte == CommandByte;
                case HandlerState.QueryFirmware:
                case HandlerState.MajorVersion:
                case HandlerState.MinorVersion:
                case HandlerState.FirmwareName:
                    return true;
                case HandlerState.StartEnd:
                    return firstByte == START_MESSAGE;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override bool HandleByte(byte messageByte)
        {
            switch (currentHandlerState)
            {
                case HandlerState.StartEnd:
                    currentHandlerState = HandlerState.StartSysex;
                    return true;
                case HandlerState.StartSysex:
                    // MAX_DATA bytes AFTER the command byte
                    currentByteCount = 0;
                    currentHandlerState = HandlerState.QueryFirmware;
                    return true;
                case HandlerState.QueryFirmware:
                    if (messageByte > 127)
                    {
                        currentHandlerState = HandlerState.StartEnd;
                        throw new MessageHandlerException(BaseExceptionMessage + "Major Version should be < 128");
                    }
                    currentHandlerState = HandlerState.MajorVersion;
                    message.MajorVersion = messageByte;
                    return true;
                case HandlerState.MajorVersion:
                    if (messageByte > 127)
                    {
                        currentHandlerState = HandlerState.StartEnd;
                        throw new MessageHandlerException(BaseExceptionMessage + "Minor Version should be < 128");
                    }
                    currentHandlerState = HandlerState.MinorVersion;
                    message.MinorVersion = messageByte;
                    return true;
                case HandlerState.MinorVersion:
                    currentHandlerState = HandlerState.FirmwareName;
                    HandleChar(messageByte);
                    return true;
                case HandlerState.FirmwareName:
                    if (messageByte == MessageConstants.SYSEX_END)
                    {
                        // Get the string we have been building all along
                        message.FirmwareName = stringBuilder.ToString();
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
