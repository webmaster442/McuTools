using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using Sharpduino.EventArguments;

namespace Sharpduino.SerialProviders
{
    public class ComPortProvider : ISerialProvider
    {
        private SerialPort port;

        public ComPortProvider(string portName, 
            int baudRate = 57600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            port = new SerialPort(portName,baudRate,parity,dataBits,stopBits);
        }

         #region Proper Dispose Code

        // Proper Dispose code should contain the following. See
        // http://stackoverflow.com/questions/538060/proper-use-of-the-idisposable-interface

        ~ComPortProvider()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool shouldDispose)
        {
            if ( shouldDispose )
            {
                // Dispose of the com port as safely as possible
                if ( port != null )
                {
                    if( port.IsOpen )
                        port.Close();
                    port.Dispose();
                    port = null;
                }
            }
        }
        #endregion

        public void Open()
        {
            port.Open();
            if (port.IsOpen)
                port.DataReceived += ComPort_DataReceived;
        }

        public void Close()
        {
            if (port.IsOpen)
            {
                port.Close();
                port.DataReceived -= ComPort_DataReceived;                    
            }
        }


        private void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] bytes = new byte[port.BytesToRead];
            port.Read(bytes, 0, bytes.Length);
            OnDataReceived(bytes);
        }

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        private void OnDataReceived(byte[] bytes)
        {
            var handler = DataReceived;
            if ( handler != null )
                handler(this,new DataReceivedEventArgs(bytes));
        }

        public void Send(IEnumerable<byte> bytes)
        {
            byte[] buffer = bytes.ToArray();
            port.Write(buffer,0,buffer.Length);
        }
    }
}