using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerPrototype;
using static System.Console;

namespace ClientServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "c/s";
            if(ReadLine()=="s")
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
                int.TryParse(ReadLine(),out int port);
                if (c.Connect(Ip, port))
                {
                    Console.WriteLine("Подключился");
                    while (true)
                    {
                        WriteLine("cmd");
                        int.TryParse(ReadLine(), out int cmd);
                        WriteLine("param");
                        string param = ReadLine();
                        c.Send((Command)cmd, param);
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
