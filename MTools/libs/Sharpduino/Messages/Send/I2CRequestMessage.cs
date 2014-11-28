using System.Collections.Generic;
using Sharpduino.Exceptions;

namespace Sharpduino.Messages.Send
{
    public class I2CRequestMessage
    {

        /* I2C read/write request
         * -------------------------------
         * 0  START_SYSEX (0xF0) (MIDI System Exclusive)
         * 1  I2C_REQUEST (0x76)
         * 2  slave address (LSB)
         * 3  slave address (MSB) + read/write and address mode bits
              {7: always 0} + {6: reserved} + {5: address mode, 1 means 10-bit mode} +
              {4-3: read/write, 00 => write, 01 => read once, 10 => read continuously, 11 => stop reading} +
              {2-0: slave address MSB in 10-bit mode, not used in 7-bit mode}
         * 4  data 0 (LSB)
         * 5  data 0 (MSB)
         * 6  data 1 (LSB)
         * 7  data 1 (MSB)
         * ...
         * n  END_SYSEX (0xF7)
         */

        private int slaveAddress;

        /// <summary>
        /// The I2C slave's address 7bits in 7-bit mode
        /// </summary>
        public int SlaveAddress
        {
            get { return slaveAddress; }
            set
            {
                if (!IsAddress10BitMode && value > 127)
                    throw new InvalidFirmataMessageException(
                        "Address should be 7bits (less than 127) when IsAddress10BitMode = false");
                slaveAddress = value;
            }
        }

        /// <summary>
        /// The addressing mode for the I2C 10bits (true) or 7bits (false)
        /// </summary>
        public bool IsAddress10BitMode { get; set; }

        /// <summary>
        /// Read/write setting
        /// </summary>
        public I2CReadWriteOptions ReadWriteOptions { get; set; }


        /// <summary>
        /// The data load we send to the slave.
        /// If we are talking for a read operation then the data is the number of bytes
        /// that we expect as an answer. For a write operation then data is a double of the
        /// register address and the value that we want to write to this register
        /// </summary>
        public List<int> Data { get; set; }

        public I2CRequestMessage()
        {
            Data = new List<int>();
            IsAddress10BitMode = false;
        }
    }
}
