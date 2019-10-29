using System;
using System.Collections.Generic;
using System.Text;

namespace ClientServerLibrary.Logger
{
    public class EmptyLogger : ILogger
    {
        public void Write(string s)
        {
            
        }

        public void WriteLine(string s)
        {
            
        }
    }
}
