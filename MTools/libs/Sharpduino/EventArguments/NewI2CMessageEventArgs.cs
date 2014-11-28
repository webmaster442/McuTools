using System;
using System.Collections.Generic;

namespace Sharpduino.EventArguments
{
    public class NewI2CMessageEventArgs : EventArgs
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
        public List<int> Data { get; set; }

        public NewI2CMessageEventArgs()
        {
            Data = new List<int>();
        }
    }
}
