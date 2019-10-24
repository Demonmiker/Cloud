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

    public class Server
    {
        Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
        Byte[] ms_buf = new Byte[10000000];
        MemoryStream ms;
        BinaryReader br;
        BinaryWriter bw;

        public void Start(int port)
        {
            ms = new MemoryStream(ms_buf);
            br = new BinaryReader(ms);
            bw = new BinaryWriter(ms);
            socket.Bind(new IPEndPoint(IPAddress.Any, 907));
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
            }
        }

        #region Handlers
        void HandleMessage()
        {
            WriteLine(br.ReadString());
            bw.Write("Сообщение получено");
            cs.Send(ms_buf);
           
        }
        

        void HandleSave()
        {
            if(Utils.SaveFile(br,"ServerData"))
            {
                bw.Write("Файл сохранен");
                cs.Send(ms_buf);
            }
            else
            {
                bw.Write("Файл не сохранен");
                cs.Send(ms_buf);
            }
            
        }

        void HandleLoad()
        {
            string path = br.ReadString();
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
