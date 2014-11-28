
namespace Sharpduino.Messages.Send

{
    public class ExtendedAnalogMessage
    {
        /// <summary>
        /// This is the pin valid values are 0-127
        /// </summary>
        public byte Pin { get; set; }

        /// <summary>
        /// This is the analog value we want to write can be up to 21 bits resolution
        /// </summary>
        public int Value { get; set; }
    }
}
