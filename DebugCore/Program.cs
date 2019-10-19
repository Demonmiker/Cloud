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
                Write("Port:");
                int.TryParse(ReadLine(),out int port);
                c.Connect(Ip, port);
                while (true)
                {
                    WriteLine("cmd");
                    int.TryParse(ReadLine(),out int cmd);
                    WriteLine("param");
                    string param = ReadLine();
                    c.Send((Command)cmd, param);
                }
            }
            ReadKey();
        }
    }
}
