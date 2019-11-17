using System;
using System.Collections.Generic;
using System.Text;

namespace ClientServerLibrary.Logger
{
    public interface ILogger
    {
        void WriteLine(string s); 

        void Write(string s);
    }
}
