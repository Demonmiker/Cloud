using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerLibrary;
using static System.Console;

namespace DebugFrame
{
    class Program
    {
        static string[] Commands;
        static void Main(string[] args)
        {
            Console.WriteLine(Environment.CurrentDirectory);
            Commands = Enum.GetNames(typeof(Command));
            Console.Title = "c/s";
            if (ReadLine() == "s")
            {
                Console.Title = "Server";
                Server s = new Server();
                Write("Port:");
                int.TryParse(ReadLine(), out int port);
                s.Start(port);
            }
            else
            {
                Console.Title = "Client";
                Client c = new Client();
                Write("IP:");
                string Ip = ReadLine();
                if (Ip == "l")
                    Ip = "127.0.0.1";
                Write("Port:");
                int.TryParse(ReadLine(), out int port);
                if (c.Connect(Ip, port))
                {
                    Console.WriteLine("Подключился");
                    while (true)
                    {
                        int CmdCode = 0;
                        WriteLine("cmd");
                        string cmd = ReadLine();
                        string[] buf = cmd.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries);

                        if (!int.TryParse(buf[0], out CmdCode))
                        {
                            for (int i = 0; i < Commands.Length; i++)
                            {
                                if (Commands[i] == buf[0])
                                {
                                    CmdCode = i;
                                }
                            }
                        }
                        if (buf.Length < 2)
                            c.Send((Command)CmdCode, string.Empty);
                        else
                            c.Send((Command)CmdCode, buf[1]);
                    }
                }
                else
                {
                    Console.WriteLine("Подключение не получилос");
                }
            }
            ReadKey();
        }
    }
}
