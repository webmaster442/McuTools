using Sharpduino.Constants;

namespace Sharpduino.Messages.Send
{
    public class ResetMessage : StaticMessage
    {
        public ResetMessage() :base(MessageConstants.SYSTEM_RESET){}
    }
}
