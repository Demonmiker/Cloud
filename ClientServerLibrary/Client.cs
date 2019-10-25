﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using static System.Console;

namespace ClientServerLibrary
{
    public partial class Client
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

        public bool Connect(String IP_Adress, int Port_Number)
        {
            try
            {
                socket.Connect(IP_Adress, Port_Number);
            }
            catch { return false; }
            return true;
            
        }

        public void Close()
        {
            socket.Close();
        }

        public void Send(Command cmd, string s)
        {
            Send(cmd, s.Split('*'));
        }

        public void Send(Command cmd, string[] param)
        {
            ms.SetLength(0);
            ms.SetLength(10000000);
            bw.Write((int)cmd);
            switch (cmd)
            {
                case Command.Message:
                    PackageMessage(param);
                    break;
                case Command.Save:
                    PackageSave(param);
                    break;
                case Command.Load:
                    PackageLoad(param);
                    break;
                case Command.Delete:
                    PackageDelete (param);
                    break;
                case Command.Rename:
                    PackageRename(param);
                    break;
                case Command.Search:
                    PackageSearch(param);
                    break;
                case Command.Move:
                    PackageMove(param);
                    break;
            }
        }

        #region Packages
        public bool PackageMessage(string[] s)
        {
            if (s.Length < 1) return false;
            bw.Write(s[0]);
            socket.Send(ms_buf);
            //
            socket.Receive(ms_buf);
            WriteLine(br.ReadString());
            return true;
        }

        public bool PackageSearch(string[] s)
        {
            if (s.Length < 1) return false;
            bw.Write(s[0]);
            socket.Send(ms_buf);
            //
            socket.Receive(ms_buf);
            string str = br.ReadString();
            str = str.Replace("?", Environment.NewLine);
            WriteLine(str);
            return true;
        }

        public bool PackageSave(string[] s)
        {
            if(s.Length<2)
                bw.Write(string.Empty);
            else
                bw.Write(s[1]);
            if (Utils.LoadFile(bw, s[0]))
            {
                
                socket.Send(ms_buf);
                //
                socket.Receive(ms_buf);
                WriteLine(br.ReadString());
                return true;
            }
            else
            {
                WriteLine("Ну далось найти файл");
                return false;
            }
            
        }

        public bool PackageLoad(string[] s)
        {
            if (s.Length < 1) return false;
            bw.Write(s[0]);
            socket.Send(ms_buf);
            //
            socket.Receive(ms_buf);
            if(br.ReadInt32()==0)
            {
                Console.WriteLine("Куда скачивать?:");
                if (!Utils.SaveFile(br, ReadLine()))
                    WriteLine("Файл не сохранен");
            }
            else
            {
                Console.WriteLine(br.ReadString());
            }
            return true;

        }
        #endregion

        ~Client()
        {
            Close();
        }
    }
}
