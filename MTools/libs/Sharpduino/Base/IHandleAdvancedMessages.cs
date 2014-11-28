using Sharpduino.Messages.Receive;

namespace Sharpduino.Base
{
    public interface IHandleAdvancedMessages :
        IHandle<AnalogMappingMessage>, IHandle<CapabilityMessage>, IHandle<CapabilitiesFinishedMessage>,
        IHandle<I2CResponseMessage>, IHandle<PinStateMessage>, IHandle<ProtocolVersionMessage>,
        IHandle<SysexFirmwareMessage>, IHandle<SysexStringMessage>
    {}
}