namespace Sharpduino.EventArguments
{
    public class NewDigitalValueEventArgs : System.EventArgs
    {
        public int Port { get; set; }
        public bool[] Pins { get; set; }
    }
}