namespace Sharpduino.Messages.Send
{
    public class I2CConfigMessage
    {
        /*
         * 2  Power pin settings (0:off or 1:on) NOT used in Standard firmata
         * 3  Delay in microseconds (LSB)
         * 4  Delay in microseconds (MSB)
         * ... user defined for special cases, etc NOT used in Standard firmata
         * n  END_SYSEX (0xF7)
         */

        //        /// <summary>
//        /// Power pin settings True:ON False:Off : This is NOT used in Standard Firmata
//        /// </summary>
//        public bool IsPowerPinOn { get; set; }

        /// <summary>
        /// The delay in microseconds
        /// </summary>
        public int Delay { get; set; }
    }
}
