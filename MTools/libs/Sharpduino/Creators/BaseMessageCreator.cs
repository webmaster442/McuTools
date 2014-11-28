namespace Sharpduino.Creators
{
    public abstract class BaseMessageCreator<T> : IMessageCreator<T> where T : class
    {
        public abstract byte[] CreateMessage(T message);
        public byte[] CreateMessage(object message)
        {
            return CreateMessage(message as T);
        }
    }
}