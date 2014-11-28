using Sharpduino.Messages.TwoWay;

namespace Sharpduino.Base
{
    public interface IHandleBasicMessages : IHandle<AnalogMessage>, IHandle<DigitalMessage>
    {}
}