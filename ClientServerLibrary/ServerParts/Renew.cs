using System;
using System.Collections.Generic;
using System.Text;

namespace ClientServerLibrary
{
    public partial class Server
    {
        public static class Renew
        {
            /// <summary>
            /// Указывает, нужно ли пытаться производить 
            /// докачку при подключении нового клиента
            /// </summary>
            public static Boolean Exist;

            /// <summary>
            /// Докачка файла
            /// </summary>
            public static void Start()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Возвращает позицию, на которой прервалась загрузка
            /// </summary>
            /// <param name="Mas"></param>
            /// <returns></returns>
            public static int GetPosition(Byte[] Mas)
            {
                int Position = Mas.Length;
                while (Mas[Position++] == 0) Position--;
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
