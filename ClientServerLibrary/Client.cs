using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using static System.Console;

namespace ClientServerPrototype
{
    public class Client
    {
        public Socket socket = new Socket(AddressFamily.InterNetwork,
          SocketType.Stream, ProtocolType.Tcp);
        Byte[] ms_buf = new Byte[10000000];
        public MemoryStream ms;
        public BinaryWriter bw;
        public BinaryReader br;

        public Client()
        {
            ms = new MemoryStream(ms_buf);
            bw = new BinaryWriter(ms);
            br = new BinaryReader(ms);
        }

        public void Connect(String IP_Adress, int Port_Number)
        {
            Console.WriteLine("Подключаюсь");
            socket.Connect(IP_Adress, Port_Number);
            Console.WriteLine("Подключился");
        }

        public void Close()
        {
            socket.Close();
        }

        public void Send(Command cmd,string s)
        {
            ms.SetLength(0);
            ms.SetLength(10000);
            bw.Write((int)cmd);
            switch (cmd)
            {
                case Command.Message:
                    PackageMessage(s);
                    break;
                case Command.Save:
                    PackageSave(s);
                    break;
                case Command.Load:
                    PackageLoad(s);
                    break;
            }

            socket.Send(ms_buf);
        }

        public void PackageMessage(string s)
        {
            bw.Write(s);
            socket.Send(ms_buf);
            //
            socket.Receive(ms_buf);
            WriteLine(br.ReadString());
        }
        
        public void PackageSave(string s)
        {
            Utils.LoadFile(bw, s);
            socket.Send(ms_buf);
            //
            socket.Receive(ms_buf);
            WriteLine(br.ReadString());
        }

        public void PackageLoad(string s)
        {
            bw.Write(s);
            socket.Send(ms_buf);
            //
            socket.Receive(ms_buf);
            Console.WriteLine("Куда скачивать?:");
            Utils.SaveFile(br, ReadLine());
        }

        ~Client()
        {
            Close();
        }
    }
}
