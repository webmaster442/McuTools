namespace Sharpduino.EventArguments
{
    public class NewAnalogValueEventArgs : System.EventArgs
    {
        public int NewValue { get; set; }
        public byte AnalogPin { get; set; }
    }
}