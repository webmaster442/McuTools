
namespace Sharpduino.Messages.Receive

{
    public class SysexFirmwareMessage
    {
        public byte MajorVersion { get; set; }
        public byte MinorVersion { get; set; }
        public string FirmwareName { get; set; }
    }
}