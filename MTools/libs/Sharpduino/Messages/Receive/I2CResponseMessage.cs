using System.Collections.Generic;

namespace Sharpduino.Messages.Receive

{
    public class I2CResponseMessage
    {
        /// <summary>
        /// The address of the I2C slave, where the response came from
        /// </summary>
        public int SlaveAddress { get; set; }

        /// <summary>
        /// The register whose value we will be reading
        /// </summary>
        public int Register { get; set; }

        /// <summary>
        /// The data at the register
        /// </summary>
        public List<int> Data { get; private set; }

        public I2CResponseMessage()
        {
            Data = new List<int>();
        }
    }
}
