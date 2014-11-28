using Sharpduino.Constants;
using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.Send;

namespace Sharpduino.Creators
{
    public class ToggleAnalogReportMessageCreator : BaseMessageCreator<ToggleAnalogReportMessage>
    {
        public override byte[] CreateMessage(ToggleAnalogReportMessage message)
        {
            if ( message == null )
                throw new MessageCreatorException("This is not a valid Toggle Analog Report Message");

            if ( message.Pin > MessageConstants.MAX_ANALOG_PINS )
                throw new MessageCreatorException("The pin should be less than " + MessageConstants.MAX_ANALOG_PINS);

            return new byte[]
                       {
                           (byte) (MessageConstants.REPORT_ANALOG_PIN | message.Pin),
                           (byte) (message.ShouldBeEnabled ? 1 : 0)
                       };
        }
    }
}
