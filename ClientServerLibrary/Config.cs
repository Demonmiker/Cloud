using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace ClientServerLibrary
{
    public class Config
    {
        #region Поля
        /// <summary>
        /// IP сервера
        /// </summary>
        public String IP_Adress;

        /// <summary>
        /// Порт, к которому выполняется подключение
        /// </summary>
        public int Port_Number;

        /// <summary>
        /// Указывает, надо ли проводить периодическую синхронизацию
        /// </summary>
        public Boolean Synchronize;

        /// <summary>
        /// Период синхронизации в секундах
        /// </summary>
        public int SycnhronizeTime;

        /// <summary>
        /// Путь к папке
        /// </summary>
        public String LocalFolderPath;
        #endregion

        #region Serialize&Deserialize
        public void Save()
        {
            JsonSerializer JS = new JsonSerializer();
            StreamWriter SW = new StreamWriter($"Config.json", false);
            try { JS.Serialize(SW, this); }
            catch (Exception E) { Console.WriteLine($"Ошибка: {E.Message}"); }
            SW.Close();
        }

        public static Config Load()
        {
            JsonSerializer JS = new JsonSerializer();
            StreamReader SR = new StreamReader("Config.json");
            Config config = null;
            try { config = (Config)JS.Deserialize(SR, typeof(Config)); }
            catch (Exception E) { Console.WriteLine($"Ошибка: {E.Message}"); }
            SR.Close();
            return config;
        }
        #endregion
    }
}
