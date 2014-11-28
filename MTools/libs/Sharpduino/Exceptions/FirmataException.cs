using System;

namespace Sharpduino.Exceptions
{
    [Serializable]
    public class FirmataException : Exception
    {
        public FirmataException(string message) : base(message){}
    }
}