using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ClientServerLibrary.Logger
{
    class ToFileLogger : ILogger
    {
        StreamWriter SW;

        public ToFileLogger()
        {
            FileStream FS = new FileStream("Log.log", FileMode.OpenOrCreate);
            SW = new StreamWriter(FS);
        }

        public void Write(String S)
        {
            SW.Write($"{DateTime.Now}: {S}");
        }

        public void WriteLine(String S)
        {
            SW.WriteLine($"{DateTime.Now}: {S}");
        }
    }
}
