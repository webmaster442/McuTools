namespace Sharpduino.Messages.Send
{
    public class ToggleDigitalReportMessage
    {
        public byte Port { get; set; }
        public bool ShouldBeEnabled { get; set; }
    }
}