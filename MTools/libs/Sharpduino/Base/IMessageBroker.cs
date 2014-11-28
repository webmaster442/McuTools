namespace Sharpduino.Base
{
    public interface IMessageBroker
    {
        void Subscribe(object handler);
        void UnSubscribe(object handler);
        void CreateEvent<T>(T message);
    }
}