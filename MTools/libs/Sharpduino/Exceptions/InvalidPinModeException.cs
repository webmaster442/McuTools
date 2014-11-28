using System;
using System.Collections.Generic;
using System.Text;
using Sharpduino.Constants;

namespace Sharpduino.Exceptions
{
    [Serializable]
    public class InvalidPinModeException : Exception
    {
        private readonly PinModes mode;
        private string availableModes;

        public InvalidPinModeException(PinModes mode, List<PinModes> availableModes )
        {
            this.mode = mode;
            var sb = new StringBuilder();
            foreach (var availableMode in availableModes)
            {
                sb.Append(availableMode);
            }
            this.availableModes = sb.ToString();
        }

        public override string Message
        {
            get { return string.Format("This pin does not support the mode {0}. Available modes are : {1}",mode,availableModes); }
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}