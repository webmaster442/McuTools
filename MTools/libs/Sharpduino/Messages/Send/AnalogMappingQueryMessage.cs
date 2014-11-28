using Sharpduino.Constants;

namespace Sharpduino.Messages.Send
{
    public class AnalogMappingQueryMessage : StaticMessage
    {
        public AnalogMappingQueryMessage() :
            base(MessageConstants.SYSEX_START, SysexCommands.ANALOG_MAPPING_QUERY, MessageConstants.SYSEX_END)
        {}
    }
}
