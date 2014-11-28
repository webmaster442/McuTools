using System.Collections.Generic;

namespace Sharpduino.Messages.Receive
{
    public class AnalogMappingMessage
    {
        /// <summary>
        /// The pinmappings list. 
        /// Each value is the analog pin and it's index is the digital pin
        /// A value of 127 means there is no analog pin equivalent
        /// </summary>
        public List<int> PinMappings { get; private set; }

        public AnalogMappingMessage()
        {
            PinMappings = new List<int>();
        }
    }
}
