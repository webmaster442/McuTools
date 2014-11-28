using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpduino.Logging
{
    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);
        void Error(string message);
        void Warn(string message);
        void Trace(string message);

        void Debug(string formatMessage, params object[] items);
        void Info(string formatMessage, params object[] items);
        void Error(string formatMessage, params object[] items);
        void Warn(string formatMessage, params object[] items);
        void Trace(string formatMessage, params object[] items);
    }
}
