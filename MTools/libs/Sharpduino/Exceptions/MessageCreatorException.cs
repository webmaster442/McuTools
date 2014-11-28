using System;

namespace Sharpduino.Exceptions
{
    [Serializable]
    public class MessageCreatorException : Exception
    {
        public MessageCreatorException(string message) : base(message){}
    }
}
