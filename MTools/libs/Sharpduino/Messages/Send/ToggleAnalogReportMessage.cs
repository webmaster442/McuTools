namespace Sharpduino.Messages.Send
{
    public class ToggleAnalogReportMessage
    {
        public byte Pin { get; set; }
        public bool ShouldBeEnabled { get; set; }
    }
}
