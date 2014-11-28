using System;
using System.Collections.Generic;
using Sharpduino.EventArguments;

namespace Sharpduino.SerialProviders
{
    /// <summary>
    /// This is an abstraction of a provider of serial data,
    /// for example a com port or some networking port
    /// </summary>
    public interface ISerialProvider : IDisposable
    {
        /// <summary>
        /// Opens the current serial provider
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the current serial provider
        /// </summary>
        void Close();

        /// <summary>
        /// This is the providers event for incoming data
        /// </summary>
        event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Use the provider to send some bytes constituting a message
        /// </summary>
        void Send(IEnumerable<byte> bytes);
    }
}
