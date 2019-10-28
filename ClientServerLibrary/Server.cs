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

    public partial class Server
    {
        Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
        Byte[] ms_buf = new Byte[108000];
        MemoryStream ms;
        BinaryReader br;
        BinaryWriter bw;
        public Config config = new Config();

        NetBuffer FNB = new NetBuffer();

        /// <summary>
        /// Перегруженый метод <see cref = "Start(int)"/> для запуска по заданому порту
        /// </summary>
        /// <param name="port"></param>
        public void Start(int port)
        {
            ms = new MemoryStream(ms_buf);
            br = new BinaryReader(ms);
            bw = new BinaryWriter(ms);
            socket.Bind(new IPEndPoint(IPAddress.Any, 907));
            socket.Listen(7);
            
            FindClient();
           
        }

        /// <summary>
        /// Перегруженый метод <see cref = "Start()"/> для запуска по данным
        /// из файла <see cref = "Config"/>.json
        /// </summary>
        public void Start()
        {
            ms = new MemoryStream(ms_buf);
            br = new BinaryReader(ms);
            bw = new BinaryWriter(ms);
            config = Config.Load();
            socket.Bind(new IPEndPoint(IPAddress.Any, config.Port_Number));
            socket.Listen(7);

            FindClient();

        }

        Socket cs;
        public void FindClient()
        {
            WriteLine("Ищу клиента");
            cs = socket.Accept();
            HandleClient();
        }

      
        private void HandleClient()
        {
            WriteLine("Нашел клиента");
            while (true)
            {
                try
                {
                    cs.Receive(ms_buf);
                }
                catch(Exception E) {WriteLine(E.Message);cs.Close(); FindClient(); };
                ms.Position = 0;
                int cmd = br.ReadInt32();
                HandleReceive((Command)cmd);
            }
        }

        private void HandleReceive(Command cmd)
        {
            switch(cmd)
            {
                case Command.Message:
                    HandleMessage();
                    break;
                case Command.Save:
                    HandleSave();
                    break;
                case Command.Load:
                    HandleLoad();
                    break;
                case Command.Delete:
                    HandleDelete();
                    break;
                case Command.Rename:
                    HandleRename();
                    break;
                case Command.Move:
                    HandleMove();
                    break;
                case Command.Search:
                    HandleSearch();
                    break;
            }
        }

        #region Handlers
        void HandleMessage()
        {
            WriteLine(br.ReadString());
            bw.Write("Сообщение получено");
            cs.Send(ms_buf);
           
        }
        public void HandleSearch()
        {
            String path = br.ReadString();
            if (path == String.Empty)
                path = "ServerData";
            else
                path = "ServerData/" + path;
            StringBuilder sb = new StringBuilder(32);
            foreach(String s in Directory.EnumerateDirectories(path))
            {
                sb.Append("D:" + s.Replace(path + '\\', "") + "?");
            }
            foreach (String s in Directory.EnumerateFiles(path))
            {
                sb.Append("F:" + s.Replace(path + "\\","") + "?");
            }
            bw.Write(sb.ToString());
            cs.Send(ms_buf);
        }



        void HandleSave()
        {
            long filesize = br.ReadInt64();
            FNB.Init(filesize+10000);
            bw.Write(true);
            cs.Send(ms_buf);
            cs.Receive(FNB.Ms_Buf);
            String path = SD(FNB.Br.ReadString());
            if (path == SD("<error>")) return;
            if (Utils.SaveFile(FNB.Br, path))
            {
                bw.Write("File Saved");
            }
            else
                bw.Write("File Save Error");
            cs.Send(ms_buf);



        }

        void HandleLoad()
        {
            String path = br.ReadString();
            path = "ServerData/" + path;
            if (Utils.CheckFile(path))
            {
                bw.Write(0);
                Utils.LoadFile(bw, path);
                cs.Send(ms_buf);
            }
            else
            {
                bw.Write(1);
                bw.Write("Не удалось получить файл");
                cs.Send(ms_buf);
            }
            
        }
        #endregion


        private String SD(String _path)
        {
            if (_path == String.Empty) return "ServerData";
            else return "ServerData/" + _path; 
        }

        public void Stop()
        {
            socket.Close();
        }

        ~Server()
        {
            Stop();
        }

    }
}
