using Sharpduino.Constants;

namespace Sharpduino.Messages.Send
{
    public class PinModeMessage
    {
        public byte Pin { get; set; }
        public PinModes Mode { get; set; }
    }
}
