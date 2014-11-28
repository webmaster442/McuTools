using System;
using System.Collections.Generic;

namespace Sharpduino.EventArguments
{
    public class DataReceivedEventArgs : EventArgs
    {
        public IEnumerable<byte> BytesReceived { get; set; }

        public DataReceivedEventArgs(IEnumerable<byte> bytesReceived)
        {
            BytesReceived = bytesReceived;
        }
    }
}