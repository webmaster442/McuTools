using Sharpduino.Constants;

namespace Sharpduino.Messages.Receive

{
    /// <summary>
    /// Message that shows the current state of one pin
    /// </summary>
    public class PinStateMessage
    {
        public int PinNo { get; set; }
        public int State { get; set; }
        public PinModes Mode { get; set; }

        public PinStateMessage()
        {
            State = 0;
            PinNo = 0;
        }
    }
}