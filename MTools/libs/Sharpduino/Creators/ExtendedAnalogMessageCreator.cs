using System.Collections.Generic;
using Sharpduino.Base;
using Sharpduino.Constants;
using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.Send;

namespace Sharpduino.Creators
{
    public class ExtendedAnalogMessageCreator : BaseMessageCreator<ExtendedAnalogMessage>
    {
        public override byte[] CreateMessage(ExtendedAnalogMessage message)
        {
            if (message == null)
                throw new MessageCreatorException("This is not an Extended Analog message");

            if (message.Pin > MessageConstants.MAX_PINS)
                throw new MessageCreatorException("Pin should be less than " + MessageConstants.MAX_PINS);

            var bytes = new List<byte> {MessageConstants.SYSEX_START, SysexCommands.EXTENDED_ANALOG,message.Pin};
            bytes.AddRange(BitHelper.GetBytesFromInt(message.Value));
            bytes.Add(MessageConstants.SYSEX_END);

            return bytes.ToArray();
        }
    }
}
