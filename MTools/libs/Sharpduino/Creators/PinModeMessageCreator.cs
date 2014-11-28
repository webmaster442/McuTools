using Sharpduino.Constants;
using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.Send;

namespace Sharpduino.Creators
{
    public class PinModeMessageCreator : BaseMessageCreator<PinModeMessage>
    {
        public override byte[] CreateMessage(PinModeMessage message)
        {
            if (message == null)
            {
                throw new MessageCreatorException("This is not a valid PinMode message");
            }

            if (message.Pin > MessageConstants.MAX_PINS)
                throw new MessageCreatorException("The pin should be less or equal to " + MessageConstants.MAX_PINS);

            return new byte[]
                       {
                           MessageConstants.SET_PIN_MODE,
                           message.Pin,
                           (byte) message.Mode
                       };
        }
    }
}
