namespace Sharpduino.Base
{
    public interface IHandle
    {
        //void Handle(object message);
    }

    public interface IHandle<T> : IHandle
    {
        void Handle(T message);
    }
}