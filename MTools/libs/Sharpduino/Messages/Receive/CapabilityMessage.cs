using System.Collections.Generic;
using Sharpduino.Constants;

namespace Sharpduino.Messages.Receive

{
    /// <summary>
    /// Message that contains the capabilities for one pin
    /// </summary>
    public class CapabilityMessage
    {
        /// <summary>
        /// This is a dictionary of the supported modes as keys
        /// which point to the appropriate resolutions
        /// </summary>
        public Dictionary<PinModes,int> Modes { get; private set; }
        public byte PinNo { get; set; }

        public CapabilityMessage()
        {
            Modes = new Dictionary<PinModes, int>();
        }
    }
}
