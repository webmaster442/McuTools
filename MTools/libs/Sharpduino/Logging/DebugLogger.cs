using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpduino.Logging
{
    public class DebugLogger : ILogger
    {
        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine("DEBUG : " + message);
        }

        public void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine("INFO : " + message);
        }

        public void Error(string message)
        {
            System.Diagnostics.Debug.WriteLine("ERROR : " + message);
        }

        public void Warn(string message)
        {
            System.Diagnostics.Debug.WriteLine("WARN : " + message);
        }

        public void Trace(string message)
        {
            System.Diagnostics.Debug.WriteLine("TRACE : " + message);
        }

        public void Debug(string formatMessage, params object[] items)
        {
            System.Diagnostics.Debug.WriteLine("DEBUG : " + string.Format(formatMessage,items));
        }

        public void Info(string formatMessage, params object[] items)
        {
            System.Diagnostics.Debug.WriteLine("INFO : " + string.Format(formatMessage, items));
        }

        public void Error(string formatMessage, params object[] items)
        {
            System.Diagnostics.Debug.WriteLine("ERROR : " + string.Format(formatMessage, items));
        }

        public void Warn(string formatMessage, params object[] items)
        {
            System.Diagnostics.Debug.WriteLine("WARN : " + string.Format(formatMessage, items));
        }

        public void Trace(string formatMessage, params object[] items)
        {
            System.Diagnostics.Debug.WriteLine("TRACE : " + string.Format(formatMessage, items));
        }
    }
}