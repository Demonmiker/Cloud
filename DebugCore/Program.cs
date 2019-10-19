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
                s.Start(907);
            }
            else
            {
                Console.Title = "Client";
                Client c = new Client();
                c.Connect("127.0.0.1", 907);
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
