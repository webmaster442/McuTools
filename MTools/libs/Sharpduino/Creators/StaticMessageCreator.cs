using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.Send;

namespace Sharpduino.Creators
{
    public class StaticMessageCreator : BaseMessageCreator<StaticMessage>
    {
        public override byte[] CreateMessage(StaticMessage message)
        {
            if (message == null)
                throw new MessageCreatorException("This is not a valid static message");
            return message.Bytes;
        }
    }
}
