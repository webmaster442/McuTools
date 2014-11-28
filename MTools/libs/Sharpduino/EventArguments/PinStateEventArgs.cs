using Sharpduino.Constants;

namespace Sharpduino.EventArguments
{
    public class PinStateEventArgs : System.EventArgs
    {
        public int Value { get; set;}
        public int Pin { get; set; }
        public PinModes Mode { get; set; }
    }
}