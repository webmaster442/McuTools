using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpduino.Logging
{
    public static class LogManager
    {
        public static ILogger CurrentLogger { get; set; }

        static LogManager()
        {
            CurrentLogger = new EmptyLogger();
        }
    }
}
