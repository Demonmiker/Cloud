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
        Byte[] ms_buf = new Byte[108000];
        public MemoryStream ms;
        public BinaryWriter bw;
        public BinaryReader br;
        public Config config = new Config();

        public NetBuffer FNB = new NetBuffer();

        public Client()
        {
            ms = new MemoryStream(ms_buf);
            bw = new BinaryWriter(ms);
            br = new BinaryReader(ms);
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
                return true;
            }
            catch { return false; }
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
            ms.SetLength(0);
            ms.SetLength(108000);
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
        public Boolean PackageMessage(String[] s)
        {
            if (s.Length < 1) return false;
            bw.Write(s[0]);
            socket.Send(ms_buf);
            //
            socket.Receive(ms_buf);
            WriteLine(br.ReadString());
            return true;
        }

        public Boolean PackageSearch(String[] s)
        {
            if (s.Length < 1) return false;
            bw.Write(s[0]);
            socket.Send(ms_buf);
            //
            socket.Receive(ms_buf);
            String str = br.ReadString();
            str = str.Replace("?", Environment.NewLine);
            WriteLine(str);
            return true;
        }

        public Boolean PackageSave(String[] s)
        {
            long filesize = Utils.FileSize(s[0]);
            if (filesize == -1) return false; 
            bw.Write(filesize);
            socket.Send(ms_buf);
            socket.Receive(ms_buf);
            if(br.ReadBoolean())
            {
                FNB.Init(filesize + 10000);
                FNB.Bw.Write(s[1]);
                if(Utils.LoadFile(FNB.Bw, s[0]))
                {
                    socket.Send(FNB.Ms_Buf);
                    socket.Receive(ms_buf);
                    WriteLine(br.ReadString());
                }
                else
                {
                    ms.Position = 0;
                    bw.Write("<error>");
                    socket.Send(ms_buf);
                    return false;
                }
                return true;
            }
            return false;
            
        }

        public Boolean PackageLoad(String[] s)
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
