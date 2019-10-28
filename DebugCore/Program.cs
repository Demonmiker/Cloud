using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerLibrary;
using static System.Console;

namespace ClientServerTest
{
    class Program
    {
        static String[] Commands;
        static void Main(String[] args)
        {
            //String IP;
            //int Port;
            Console.WriteLine(Environment.CurrentDirectory);
            Commands = Enum.GetNames(typeof(Command));
            Console.Title = "c/s";

            #region Server
            if (ReadLine() == "s")
            {
                Console.Title = "Server";
                Server s = new Server();
                int port = 0;
                try { s.Start(); }
                catch
                {
                    Write("Port:");
                    int.TryParse(ReadLine(), out port);
                    s.config.Port_Number = port;
                    s.Start();
                }
            }
            #endregion Server

            #region Client
            else
            {
                Console.Title = "Client";
                Client c = new Client();
                if (!c.LoadFromConfig())
                {
                    Write("IP:");
                    String Ip = ReadLine();
                    if (Ip == "l")
                        Ip = "127.0.0.1";
                    Write("Port:");
                    int.TryParse(ReadLine(), out int Port);
                    c.config.IP_Adress = Ip;
                    c.config.Port_Number = Port;
                }
                if (c.Connect())
                {
                    Console.WriteLine("Подключился");
                    while (true)
                    {
                        int CmdCode = 0;
                        WriteLine("cmd");
                        String cmd = ReadLine();
                        String[] buf = cmd.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries);

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
                            c.Send((Command)CmdCode, String.Empty);
                        else
                            c.Send((Command)CmdCode, buf[1]);
                    }
                }
                else
                {
                    Console.WriteLine("Подключение не получилос");
                }
            }
            #endregion Client
            ReadKey();
        }
    }
}
