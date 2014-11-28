using System.Collections.Generic;
using Sharpduino.Constants;

namespace Sharpduino
{
    public class Pin
    {
        public PinModes CurrentMode { get; set; }
        public Dictionary<PinModes, int> Capabilities { get; set; }
        public int CurrentValue { get; set; }

        public Pin()
        {
            Capabilities = new Dictionary<PinModes, int>();
            CurrentValue = 0;
        }
    }
}