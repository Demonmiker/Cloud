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
        public bool PackageDelete(string[] s)
        {
            if (s.Length < 1) return false;
            bw.Write(s[0]);
            socket.Send(ms_buf);
            socket.Receive(ms_buf);
            WriteLine(br.ReadString());
            return true;
        }

        public bool PackageMove(string[] s)
        {
            if (s.Length < 2) return false;
            bw.Write(s[0]);
            bw.Write(s[1]);
            socket.Send(ms_buf);
            socket.Receive(ms_buf);
            WriteLine(br.ReadString());
            return true;
        }
        public bool PackageRename(string[] s)
        {
            if (s.Length < 2) return false;
            bw.Write(s[0]);
            bw.Write(s[1]);
            socket.Send(ms_buf);
            socket.Receive(ms_buf);
            WriteLine(br.ReadString());
            return true;
        }
    
    }
}
