using System;
using System.Collections.Generic;
using System.Text;

namespace ClientServerLibrary.Logger
{
    class CombinedLogger : ILogger
    {
        ILogger[] Loggers;

        public CombinedLogger(ILogger[] Loggers) { }

        public void Write(String S)
        {
            foreach (ILogger logger in Loggers)
                logger.Write(S);
        }

        public void WriteLine(String S)
        {
            foreach (ILogger logger in Loggers)
                logger.WriteLine(S);
        }
    }
}
