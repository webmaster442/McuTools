using System;

namespace Sharpduino.Exceptions
{
    [Serializable]
    public class InvalidFirmataMessageException : Exception
    {
        public InvalidFirmataMessageException(string message) : base(message)
        {
            
        }
    }
}