using Sharpduino.Constants;

namespace Sharpduino.Messages.Send
{
    public class QueryFirmwareMessage : StaticMessage
    {
        public QueryFirmwareMessage() : 
            base(MessageConstants.SYSEX_START,SysexCommands.QUERY_FIRMWARE,MessageConstants.SYSEX_END)
        {}
    }
}
