using Sharpduino.Constants;

namespace Sharpduino.Messages.Send
{
    public class QueryCapabilityMessage : StaticMessage
    {
        public QueryCapabilityMessage() : 
            base(MessageConstants.SYSEX_START,SysexCommands.CAPABILITY_QUERY,MessageConstants.SYSEX_END)
        {}
    }
}
