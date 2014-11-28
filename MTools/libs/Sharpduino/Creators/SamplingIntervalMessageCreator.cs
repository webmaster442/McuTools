using System.Collections.Generic;
using Sharpduino.Base;
using Sharpduino.Constants;
using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.Send;

namespace Sharpduino.Creators
{
    public class SamplingIntervalMessageCreator : BaseMessageCreator<SamplingIntervalMessage>
    {
        public override byte[] CreateMessage(SamplingIntervalMessage message)
        {
            if (message == null)
                throw new MessageCreatorException("This is not a valid Sampling Interval Message");

            var bytes = new List<byte>()
                       {
                           MessageConstants.SYSEX_START,
                           SysexCommands.SAMPLING_INTERVAL,
                       };
            bytes.AddRange(BitHelper.GetBytesFromInt(message.Interval));
            bytes.Add(MessageConstants.SYSEX_END);

            return bytes.ToArray();
        }
    }
}
