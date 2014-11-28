using Sharpduino.Base;
using Sharpduino.Constants;
using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.TwoWay;

namespace Sharpduino.Creators
{
    public class DigitalMessageCreator : BaseMessageCreator<DigitalMessage>
    {
        public override byte[] CreateMessage(DigitalMessage message)
        {
            if (message == null)
            {
                throw new MessageCreatorException("This is not a digital message");
            }

            byte lsb, msb;
            BitHelper.IntToBytes(BitHelper.PinVals2PortVal(message.PinStates), out lsb, out msb);

            return new byte[]
                            {
                                (byte) (MessageConstants.DIGITAL_MESSAGE | message.Port),
                                lsb, 
                                msb
                            };
        }
    }
}
