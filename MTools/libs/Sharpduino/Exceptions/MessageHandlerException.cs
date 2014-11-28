using System;

namespace Sharpduino.Exceptions
{
    [Serializable]
    public class MessageHandlerException : Exception
    {
        public MessageHandlerException(string message) : base(message)
        {
        }
    }
}