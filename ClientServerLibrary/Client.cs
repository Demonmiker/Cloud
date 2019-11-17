using System;
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
        NetBuffer CNB = new NetBuffer();
        public Config config = new Config();

        public NetBuffer FNB = new NetBuffer();

        public Client()
        {
            CNB.Init(100000);
        }

        public Boolean LoadFromConfig()
        {
            try
            {
                config = Config.Load();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Перегрузка метода <see cref = "Connect(String, int)"/> для подключения по данным,
        /// которые ввёл пользователь
        /// </summary>
        /// <param name="IP_Adress"></param>
        /// <param name="Port_Number"></param>
        /// <returns></returns>
        public Boolean Connect(String IP_Adress, int Port_Number)
        {
            try
            {
                socket.Connect(IP_Adress, Port_Number);
                Renew();
            }
            catch { return false; }
            return true;
            
        }

        /// <summary>
        /// Перегрузка метода <see cref = "Connect()"/> для подключения по данным
        /// из файла <see cref = "Config"/>.json
        /// </summary>
        /// <returns></returns>
        public Boolean Connect()
        {
            try
            {
                socket.Connect(config.IP_Adress, config.Port_Number);
                Renew();
                return true;
            }
            catch { return false; }
        }

        private void Renew()
        {
            socket.Receive(CNB.Ms_Buf);
            if (CNB.Br.ReadBoolean())
            {
                WriteLine("Произвожу докачку...");
                //
                // Докачка
                //
                String[] Param = new String[2];
                PackageSave(Param, 0);
                //
                // Докачка закончена
                //
                WriteLine("Докачка закончена");
            }
            else WriteLine("Докачка не требуется");
        }

        public void Close()
        {
            socket.Close();
        }

        public void Send(Command cmd, String s)
        {
            Send(cmd, s.Split('*'));
        }

        public void Send(Command cmd, String[] param)
        {
            CNB.Clear();
            CNB.Bw.Write((int)cmd);
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
        public Boolean PackageMessage(String[] s)
        {
            if (s.Length < 1) return false;
            CNB.Bw.Write(s[0]);
            socket.Send(CNB.Ms_Buf);
            //
            socket.Receive(CNB.Ms_Buf);
            WriteLine(CNB.Br.ReadString());
            return true;
        }

        public Boolean PackageSearch(String[] s)
        {
            if (s.Length < 1) return false;
            CNB.Bw.Write(s[0]);
            socket.Send(CNB.Ms_Buf);
            //
            socket.Receive(CNB.Ms_Buf);
            String str = CNB.Br.ReadString();
            str = str.Replace("?", Environment.NewLine);
            WriteLine(str);
            return true;
        }

        public Boolean PackageSave(String[] s)
        {
            long filesize = Utils.FileSize(s[0]);
            if (filesize == -1) return false; 
            CNB.Bw.Write(filesize);
            socket.Send(CNB.Ms_Buf);
            socket.Receive(CNB.Ms_Buf);
            if(CNB.Br.ReadBoolean())
            {
                FNB.Init(filesize + 10000);
                FNB.Bw.Write(s[1]);
                if(Utils.LoadFile(FNB.Bw, s[0]))
                {
                    socket.Send(FNB.Ms_Buf);
                    socket.Receive(CNB.Ms_Buf);
                    WriteLine(CNB.Br.ReadString());
                }
                else
                {
                    CNB.Ms.Position = 0;
                    CNB.Bw.Write("<error>");
                    socket.Send(CNB.Ms_Buf);
                    return false;
                }
                return true;
            }
            return false;
            
        }

        public Boolean PackageSave(String[] s, int Position)
        {
            long filesize = Utils.FileSize(s[0]);
            if (filesize == -1) return false;
            CNB.Bw.Write(filesize);
            socket.Send(CNB.Ms_Buf);
            socket.Receive(CNB.Ms_Buf);
            if (CNB.Br.ReadBoolean())
            {
                FNB.Init(filesize + 10000);
                FNB.Bw.Write(s[1]);
                if (Utils.LoadFile(FNB.Bw, s[0], Position))
                {
                    socket.Send(FNB.Ms_Buf);
                    socket.Receive(CNB.Ms_Buf);
                    WriteLine(CNB.Br.ReadString());
                }
                else
                {
                    CNB.Ms.Position = 0;
                    CNB.Bw.Write("<error>");
                    socket.Send(CNB.Ms_Buf);
                    return false;
                }
                return true;
            }
            return false;

        }

        public Boolean PackageLoad(String[] s)
        {
            if (s.Length < 2) return false;
            CNB.Bw.Write(s[0]);
            socket.Send(CNB.Ms_Buf);
            socket.Receive(CNB.Ms_Buf);
            long size = CNB.Br.ReadInt64();
            if (size == -1) return false;
            FNB.Init(size + 10000);
            socket.Receive(FNB.Ms_Buf);
            if (Utils.SaveFile(FNB.Br, s[1]))
            {
                WriteLine("Успешно загружен");
                return true;
            }
            else
            {
                WriteLine("Ошибка сохранения");
                return false;
            }


        }

        public bool PackageDelete(string[] s)
        {
            if (s.Length < 1) return false;
            CNB.Bw.Write(s[0]);
            socket.Send(CNB.Ms_Buf);
            socket.Receive(CNB.Ms_Buf);
            WriteLine(CNB.Br.ReadString());
            return true;
        }

        public bool PackageMove(string[] s)
        {
            if (s.Length < 2) return false;
            CNB.Bw.Write(s[0]);
            CNB.Bw.Write(s[1]);
            socket.Send(CNB.Ms_Buf);
            socket.Receive(CNB.Ms_Buf);
            WriteLine(CNB.Br.ReadString());
            return true;
        }
        public bool PackageRename(string[] s)
        {
            if (s.Length < 2) return false;
            CNB.Bw.Write(s[0]);
            CNB.Bw.Write(s[1]);
            socket.Send(CNB.Ms_Buf);
            socket.Receive(CNB.Ms_Buf);
            WriteLine(CNB.Br.ReadString());
            return true;
        }
        #endregion

        ~Client()
        {
            Close();
        }
    }
}
