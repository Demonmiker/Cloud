﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using static System.Console;
using ClientServerLibrary.Logger;
using ClientServerLibrary;


namespace ClientServerLibrary
{
    public partial class Server
    {
        #region Поля
        /// <summary>
        /// <see cref="Socket"/>, через который осуществляется подключение
        /// </summary>
        Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
        
        /// <summary>
        /// Конфиг сервера
        /// </summary>
        public Config config = new Config();

        /// <summary>
        /// <see cref="NetBuffer"/>, в который получаем данные от клиента
        /// </summary>
        NetBuffer CNB = new NetBuffer();

        /// <summary>
        /// <see cref="NetBuffer"/>, в который загружаем данные
        /// для последующей отправки клиенту
        /// </summary>
        NetBuffer FNB = new NetBuffer();

        /// <summary>
        /// <see cref="ILogger"/>, выводящий данные в консоль и
        /// (опционально) .log файл 
        /// </summary>
        ILogger log = new ConsoleLogger();
        #endregion

        /// <summary>
        /// Перегруженый метод <see cref = "Start(int)"/> для запуска по заданому порту
        /// </summary>
        /// <param name="port"></param>
        public void Start(int port)
        {
            log.WriteLine("Инициализация");            
            CNB.Init(100000);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(7);
            log.WriteLine("Сервер включен");
            FindClient();
        }

        /// <summary>
        /// Перегруженый метод <see cref = "Start()"/> для запуска по данным
        /// из файла <see cref = "Config"/>.json
        /// </summary>
        public void Start()
        {
            log.WriteLine("Инициализация");
            CNB.Init(100000);
            config = Config.Load();
            socket.Bind(new IPEndPoint(IPAddress.Any, config.Port_Number));
            socket.Listen(7);
            log.WriteLine("Сервер включен");
            FindClient();
        }

        Socket cs;
        
        
        public void FindClient()
        {
            log.WriteLine("Ищу клиента");
            cs = socket.Accept(); // Приняли подключение
            log.WriteLine("Проверяю возможность и необходимость докачки");
            // Производим докачку если она нужна и таймаут ещё не закончился
            if (Server.Renew.Need)
            {
                if ((DateTime.Now - Renew.ClientLosenTime).TotalSeconds < 35)
                // в этом месте в последствии "35" нужно будет заменить на
                // специальный параметр "RenewTimeOut"
                {
                    Renew.Start(CNB, cs, log);
                    cs.Receive(CNB.Ms_Buf);
                    CNB.Ms.Position = 0;
                    HandleSave();
                }
                else
                {
                    log.WriteLine("Время для докачки вышло");
                    Renew.Need = false;
                    CNB.Bw.Write(Renew.Need);
                    cs.Send(CNB.Ms_Buf);
                }
            }
            else
            {
                log.WriteLine("Докачка не нужна");
                CNB.Bw.Write(Renew.Need);
                cs.Send(CNB.Ms_Buf);
            }
            // Далее в любом случае начинаем ждать данных от клиента для обработки
            HandleClient();
        }
      
        /// <summary>
        /// Получение и обработка данных от клиента
        /// </summary>
        private void HandleClient()
        {
            int cmd = 0;
            log.WriteLine("Нашел клиента");
            while (true)
            {
                try
                {
                    cs.Receive(CNB.Ms_Buf);
                    CNB.Ms.Position = 0;
                    // Получаем команду и вызываем метод, выбирающий нужный обработчик
                    cmd = CNB.Br.ReadInt32();
                    HandleReceive((Command)cmd);
                }
                catch (Exception E)
                {
                    WriteLine(E.Message);
                    cs.Close();
                    Command c = (Command)cmd;
                    // Если что-то пошло не так во время загрузки файла на сервер, то
                    // устанавливаем сервер в режим ожидания переподключения клиента
                    if (CNB.Ms_Buf.Length != 0 && c == Command.Save)
                    {
                        Renew.ClientLosenTime = DateTime.Now;
                        Renew.ExecCommandParam = CNB.Br.ReadString();
                        Renew.BuffPosition = Renew.GetPosition(CNB.Ms_Buf);
                        Renew.Need = true;
                    }
                    // Далее в любом случае начинаем заново ждать подключения
                    FindClient();
                }
            }
        }

        /// <summary>
        /// Вызов нужного для данной команды обработчика
        /// </summary>
        /// <param name="cmd"></param>
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
            log.Write("Сообщение: ");
            log.Write(CNB.Br.ReadString());
            CNB.Bw.Write("Сообщение получено");//клиенту
            cs.Send(CNB.Ms_Buf);
            log.WriteLine("!");

        }
        public void HandleSearch()
        {
            log.Write("Поиск: ");
            String path = CNB.Br.ReadString();
            path = SD(path);
            log.WriteLine(path);
            StringBuilder sb = new StringBuilder(32);
            if(!Directory.Exists(path))
            {
                CNB.Bw.Write("Папка не найдена");
                cs.Send(CNB.Ms_Buf);
            }
            else
            {
                foreach (String s in Directory.EnumerateDirectories(path))
                {
                    sb.Append("D:" + s.Replace(path + '\\', "") + "?");
                }
                foreach (String s in Directory.EnumerateFiles(path))
                {
                    sb.Append("F:" + s.Replace(path + "\\", "") + "?");
                }
                CNB.Bw.Write(sb.ToString());//клиенту
                cs.Send(CNB.Ms_Buf);
            }
           
            log.WriteLine("!");
        }



        void HandleSave()
        {
            log.Write("Сохранение: ");
            long filesize = CNB.Br.ReadInt64();
            FNB.Init(filesize+10000);
            CNB.Bw.Write(true);
            cs.Send(CNB.Ms_Buf);
            cs.Receive(FNB.Ms_Buf);
            String path = SD(FNB.Br.ReadString());
            if (path == SD("<error>")) { log.WriteLine("ошибка у клиента");  return; }
            log.Write(path);
            if (Utils.SaveFile(FNB.Br, path))
            {
                log.Write(" сохранен");
                CNB.Bw.Write("Файл сохранен");//клиенту
            }
            else
            {
                log.Write(" не сохранен");
                CNB.Bw.Write("Файл не сохранен");//клиенту
            }
                
            cs.Send(CNB.Ms_Buf);
            log.WriteLine("!");



        }

        void HandleLoad()
        {
            log.Write("Загрузка: ");
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
            log.WriteLine("!");
        }

        void HandleDelete()
        {
            log.Write("Удаление: ");
            FileInfo FI = null;
            DirectoryInfo DI = null;
            String path = CNB.Br.ReadString();
            path = "ServerData/" + path;
            log.Write(path);
            string answer = "@@@";
            if (path.Contains('.')) FI = new FileInfo(path);
            else DI = new DirectoryInfo(path);
            if (FI != null) try
                {
                    FI.Delete();
                    answer= "Файл успешно удалён";
                }
                catch (Exception E) { answer= $"Не удалось переместить файл - {E.Message}"; }
            else try
                {
                    DI.Delete();
                    answer = "Директория успешно удалена";
                }
                catch (Exception E) { answer= $"Не удалось переместить директорию - {E.Message}"; }
            log.Write(answer);
            CNB.Bw.Write(answer);
            cs.Send(CNB.Ms_Buf);
            log.WriteLine("!");
        }

        void HandleMove()
        {
            log.Write("Перемещение: ");
            FileInfo FI = null;
            DirectoryInfo DI = null;
            String path = CNB.Br.ReadString();
            path = "ServerData/" + path;
            string answer = "###";
            if (path.Contains('.')) FI = new FileInfo(path);
            else DI = new DirectoryInfo(path);
            if (FI != null) try
                {
                    FI.MoveTo(SD($"{CNB.Br.ReadString()}/{FI.Name}"));
                    answer = "Файл успешно перемещён";
                }
                catch (Exception E) { answer = $"Не удалось переместить файл - {E.Message}"; }
            else try
                {
                    DI.MoveTo(SD($"{CNB.Br.ReadString()}/{DI.Name}"));
                    answer = "Директория успешно перемещена";
                }
                catch (Exception E) {answer = $"Не удалось переместить директорию - {E.Message}"; }
            log.Write(answer);
            CNB.Bw.Write(answer);
            cs.Send(CNB.Ms_Buf);
            log.WriteLine("!");
        }

        void HandleRename()
        {
            log.Write("Перемещение: ");
            FileInfo FI = null;
            DirectoryInfo DI = null;
            String path = CNB.Br.ReadString();
            path = "ServerData/" + path;
            log.Write(path);
            string answer = "$$$";
            string To = CNB.Br.ReadString();
            log.Write(" " + To);
            if (path.Contains('.')) FI = new FileInfo(path);
            else DI = new DirectoryInfo(path);
            if (FI != null) try
                {
                    FI.MoveTo($"{(FI.DirectoryName == "ServerData" ? "" : FI.DirectoryName)}/{To}");
                    answer = "Файл успешно переименован";
                }
                catch (Exception E) { answer = $"Не удалось переименовать файл - {E.Message}"; }
            else try
                {
                    DI.MoveTo($"{DI.Parent.Name}/{To}");
                    answer = "Директория успешно переименована";
                }
                catch (Exception E) { answer=$"Не удалось переименовать директорию - {E.Message}"; }
            CNB.Bw.Write(answer);
            cs.Send(CNB.Ms_Buf);
            log.WriteLine("!");
        }
        #endregion

        /// <summary>
        /// Возвразает путь относительно папки "ServerData"
        /// <para/> (При пустой строке возвращает "ServerData")
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        private String SD(String _path)
        {
            if (_path == String.Empty) return "ServerData";
            else return "ServerData/" + _path; 
        }

        /// <summary>
        /// Закрытие сокета
        /// </summary>
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
