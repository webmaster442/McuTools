using Sharpduino.Constants;
using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.Send;

namespace Sharpduino.Creators
{
    public class PinStateQueryMessageCreator : BaseMessageCreator<PinStateQueryMessage>
    {
        public override byte[] CreateMessage(PinStateQueryMessage message)
        {
            if (message == null)
                throw new MessageCreatorException("This is not a valid PinState Query Message");

            if (message.Pin > MessageConstants.MAX_PINS)
                throw new MessageCreatorException("Pin should be less than " + MessageConstants.MAX_PINS);

            return new byte[]
                       {
                           MessageConstants.SYSEX_START,
                           SysexCommands.PIN_STATE_QUERY,
                           message.Pin,
                           MessageConstants.SYSEX_END
                       };
        }
    }
}
