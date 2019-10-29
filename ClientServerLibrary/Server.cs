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
        
        public Config config = new Config();

        NetBuffer CNB = new NetBuffer();
        NetBuffer FNB = new NetBuffer();

        /// <summary>
        /// Перегруженый метод <see cref = "Start(int)"/> для запуска по заданому порту
        /// </summary>
        /// <param name="port"></param>
        public void Start(int port)
        {
            CNB.Init(100000);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(7);
            FindClient();
        }

        /// <summary>
        /// Перегруженый метод <see cref = "Start()"/> для запуска по данным
        /// из файла <see cref = "Config"/>.json
        /// </summary>
        public void Start()
        {
            CNB.Init(100000);
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
                    cs.Receive(CNB.Ms_Buf);
                    CNB.Ms.Position = 0;
                    int cmd = CNB.Br.ReadInt32();
                    HandleReceive((Command)cmd);
                }
                catch (Exception E) { WriteLine(E.Message); cs.Close(); FindClient(); };
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
            WriteLine(CNB.Br.ReadString());
            CNB.Bw.Write("Сообщение получено");
            cs.Send(CNB.Ms_Buf);
           
        }
        public void HandleSearch()
        {
            String path = CNB.Br.ReadString();
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
            CNB.Bw.Write(sb.ToString());
            cs.Send(CNB.Ms_Buf);
        }



        void HandleSave()
        {
            long filesize = CNB.Br.ReadInt64();
            FNB.Init(filesize+10000);
            CNB.Bw.Write(true);
            cs.Send(CNB.Ms_Buf);
            cs.Receive(FNB.Ms_Buf);
            String path = SD(FNB.Br.ReadString());
            if (path == SD("<error>")) return;
            if (Utils.SaveFile(FNB.Br, path))
            {
                CNB.Bw.Write("File Saved");
            }
            else
                CNB.Bw.Write("File Save Error");
            cs.Send(CNB.Ms_Buf);



        }

        void HandleLoad()
        {
            string path = SD(CNB.Br.ReadString());
            long size = Utils.FileSize(path);
            CNB.Bw.Write(size);
            cs.Send(CNB.Ms_Buf);
            if (size == -1) return;
            FNB.Init(size + 10000);
            if (Utils.LoadFile(FNB.Bw, path))
            {
                cs.Send(FNB.Ms_Buf);
            }
        }

        void HandleDelete()
        {
            FileInfo FI = null;
            DirectoryInfo DI = null;
            String path = CNB.Br.ReadString();
            path = "ServerData/" + path;
            if (path.Contains('.')) FI = new FileInfo(path);
            else DI = new DirectoryInfo(path);
            if (FI != null) try
                {
                    FI.Delete();
                    CNB.Bw.Write("Файл успешно удалён");
                }
                catch (Exception E) { CNB.Bw.Write($"Не удалось переместить файл - {E.Message}"); }
            else try
                {
                    DI.Delete();
                    CNB.Bw.Write("Директория успешно удалена");
                }
                catch (Exception E) { CNB.Bw.Write($"Не удалось переместить директорию - {E.Message}"); }
            cs.Send(CNB.Ms_Buf);
        }

        void HandleMove()
        {
            FileInfo FI = null;
            DirectoryInfo DI = null;
            String path = CNB.Br.ReadString();
            path = "ServerData/" + path;
            if (path.Contains('.')) FI = new FileInfo(path);
            else DI = new DirectoryInfo(path);
            if (FI != null) try
                {
                    FI.MoveTo(SD($"{CNB.Br.ReadString()}/{FI.Name}"));
                    CNB.Bw.Write("Файл успешно перемещён");
                }
                catch (Exception E) { CNB.Bw.Write($"Не удалось переместить файл - {E.Message}"); }
            else try
                {
                    DI.MoveTo(SD($"{CNB.Br.ReadString()}/{DI.Name}"));
                    CNB.Bw.Write("Директория успешно перемещена");
                }
                catch (Exception E) { CNB.Bw.Write($"Не удалось переместить директорию - {E.Message}"); }
            cs.Send(CNB.Ms_Buf);
        }

        void HandleRename()
        {
            FileInfo FI = null;
            DirectoryInfo DI = null;
            String path = CNB.Br.ReadString();
            path = "ServerData/" + path;
            if (path.Contains('.')) FI = new FileInfo(path);
            else DI = new DirectoryInfo(path);
            if (FI != null) try
                {
                    FI.MoveTo($"{(FI.DirectoryName == "ServerData" ? "" : FI.DirectoryName)}/{CNB.Br.ReadString()}");
                    CNB.Bw.Write("Файл успешно переименован");
                }
                catch (Exception E) { CNB.Bw.Write($"Не удалось переименовать файл - {E.Message}"); }
            else try
                {
                    DI.MoveTo($"{DI.Parent.Name}/{CNB.Br.ReadString()}");
                    CNB.Bw.Write("Директория успешно переименована");
                }
                catch (Exception E) { CNB.Bw.Write($"Не удалось переименовать директорию - {E.Message}"); }
            cs.Send(CNB.Ms_Buf);
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
