using System;
using System.Collections.Generic;
using System.Text;

namespace ClientServerLibrary.Logger
{
    public class ConsoleLogger : ILogger
    {
        public void Write(string s)
        {
            Console.Write(s);
        }

        public void WriteLine(string s)
        {
            Console.WriteLine(s);
        }
    }
}
