using Sharpduino.Base;
using Sharpduino.Constants;
using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.TwoWay;

namespace Sharpduino.Creators
{
    public class AnalogOutputMessageCreator : BaseMessageCreator<AnalogMessage>
    {
        public override byte[] CreateMessage(AnalogMessage message)
        {
            if (message == null)
            {
                throw new MessageCreatorException("This is not an analog message");
            }
            
            byte lsb, msb;
            BitHelper.IntToBytes(message.Value, out lsb, out msb);
            // TODO : see if the value should have any constraints
            return new byte[]
                       {
                           (byte) (MessageConstants.ANALOG_MESSAGE | message.Pin),
                           lsb, 
                           msb,
                       };
        }
    }
}