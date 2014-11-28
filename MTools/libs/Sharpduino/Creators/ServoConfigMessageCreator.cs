using System.Collections.Generic;
using Sharpduino.Base;
using Sharpduino.Constants;
using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.Send;

namespace Sharpduino.Creators
{
    public class ServoConfigMessageCreator : BaseMessageCreator<ServoConfigMessage>
    {
        public override byte[] CreateMessage(ServoConfigMessage message)
        {
            if (message == null)
                throw new MessageCreatorException("This is not a valid Servo Config Message");

            var bytes = new List<byte>
                            {
                                MessageConstants.SYSEX_START,
                                SysexCommands.SERVO_CONFIG,
                                message.Pin
                            };
            bytes.AddRange(BitHelper.GetBytesFromInt(message.MinPulse));
            bytes.AddRange(BitHelper.GetBytesFromInt(message.MaxPulse));
            bytes.AddRange(BitHelper.GetBytesFromInt(message.Angle));
            bytes.Add(MessageConstants.SYSEX_END);

            return bytes.ToArray();
        }
    }
}
