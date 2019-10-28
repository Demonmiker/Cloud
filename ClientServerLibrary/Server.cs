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

        NetBuffer FNB = new NetBuffer();

        public void Start(int port)
        {
            ms = new MemoryStream(ms_buf);
            br = new BinaryReader(ms);
            bw = new BinaryWriter(ms);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
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
            string path = br.ReadString();
            if (path == string.Empty)
                path = "ServerData";
            else
                path = "ServerData/" + path;
            StringBuilder sb = new StringBuilder(32);
            foreach(string s in Directory.EnumerateDirectories(path))
            {
                sb.Append("D:" + s.Replace(path + '\\', "") + "?");
            }
            foreach (string s in Directory.EnumerateFiles(path))
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
            string path = SD(FNB.Br.ReadString());
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
            string path = SD(br.ReadString());
            long size = Utils.FileSize(path);
            bw.Write(size);
            cs.Send(ms_buf);
            if (size == -1) return;
            FNB.Init(size + 10000);
            if(Utils.LoadFile(FNB.Bw, path))
            {
                cs.Send(FNB.Ms_Buf);
            }
            
        }
        #endregion


        private string SD(string _path)
        {
            if (_path == string.Empty) return "ServerData";
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
