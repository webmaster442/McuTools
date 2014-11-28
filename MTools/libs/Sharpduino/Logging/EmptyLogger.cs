using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpduino.Logging
{
    public class EmptyLogger : ILogger
    {
        public void Debug(string message){}
        public void Info(string message){}
        public void Error(string message){}
        public void Warn(string message){}
        public void Trace(string message){}

        public void Debug(string formatMessage, params object[] items){}
        public void Info(string formatMessage, params object[] items){}       
        public void Error(string formatMessage, params object[] items){}      
        public void Warn(string formatMessage, params object[] items){}
        public void Trace(string formatMessage, params object[] items){}
        
    }
}
