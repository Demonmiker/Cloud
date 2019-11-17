using ClientServerLibrary.Logger;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ClientServerLibrary
{
    public partial class Server
    {
        /// <summary>
        /// Докачка
        /// </summary>
        public static class Renew
        {
            /// <summary>
            /// Указывает, нужно ли пытаться производить 
            /// докачку при подключении нового клиента
            /// </summary>
            public static Boolean Need;

            /// <summary>
            /// Докачка файла
            /// </summary>
            public static void Start(NetBuffer CNB, Socket cs, ILogger log)
            {
                //throw new NotImplementedException();
                CNB.Bw.Write(Need);
                CNB.Bw.Write(ExecCommandParam);
                CNB.Bw.Write(BuffPosition);
                log.WriteLine("Клиент начинает докачку");
                cs.Send(CNB.Ms_Buf);
                log.WriteLine("Клиент завершил докачку");
            }

            /// <summary>
            /// Возвращает позицию, на которой прервалась загрузка
            /// </summary>
            /// <param name="Mas"></param>
            /// <returns></returns>
            public static int GetPosition(Byte[] Mas)
            {
                int Position = Mas.Length - 1;
                while (Mas[Position--] == 0);
                return Position;
            }

            /// <summary>
            /// Время последнего аварийного отключения клиента
            /// <para/> Нужно для "докачки"
            /// </summary>
            public static DateTime ClientLosenTime;

            /// <summary>
            /// Команда, выполнение которой было прервано
            /// последним аварийным отключением клиента
            /// </summary>
            public static Command ExecCommand;

            /// <summary>
            /// Путь к файлу, работа с которым была прервана
            /// последним аварийным отключением клиента
            /// </summary>
            public static String ExecCommandParam;

            /// <summary>
            /// Позиция, до которой клиент успел загрузить представляющий файл
            /// бинарный массив перед последним аварийным отключением
            /// </summary>
            public static int BuffPosition;
        }
    }
}
