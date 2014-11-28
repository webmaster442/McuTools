using Sharpduino.Constants;

namespace Sharpduino.Messages.Send
{
    public class ProtocolVersionRequestMessage : StaticMessage
    {
        public ProtocolVersionRequestMessage() : 
            base(new byte[]{MessageConstants.PROTOCOL_VERSION})
        {}
    }
}
