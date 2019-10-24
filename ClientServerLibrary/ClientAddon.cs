using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using static System.Console;

namespace ClientServerLibrary
{
    public partial class Client
    {
        public bool PackageDelete(string s)
        {
            bw.Write(s);
            socket.Send(ms_buf);
            socket.Receive(ms_buf);
            WriteLine(br.ReadString());
            return true;
        }

        public bool PackageMove(string s)
        {
            bw.Write(s);
            WriteLine("Введите новое расположение файла");
            s = ReadLine();
            bw.Write(s);
            socket.Send(ms_buf);
            socket.Receive(ms_buf);
            WriteLine(br.ReadString());
            return true;
        }
        public bool PackageRename(string s)
        {
            bw.Write(s);
            WriteLine("Введите новое имя файла");
            s = ReadLine();
            bw.Write(s);
            socket.Send(ms_buf);
            socket.Receive(ms_buf);
            WriteLine(br.ReadString());
            return true;
        }
    
    }
}
