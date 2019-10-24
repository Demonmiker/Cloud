using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace ClientServerLibrary
{
    partial class Server
    {
        void HandleDelete()
        {
            string path = br.ReadString();
            path = "ServerData/" + path;
            FileInfo fi = new FileInfo(path);
            try
            {
                fi.Delete();
                bw.Write("Файл успешно удалён");
            }
            catch (Exception E) { bw.Write($"Не удалось удалить файл - {E.Message}"); }
            cs.Send(ms_buf);
        }

        void HandleMove()
        {
            string path = br.ReadString();
            path = "ServerData/" + path;
            FileInfo fi = new FileInfo(path);
            try
            {
                fi.MoveTo(br.ReadString());
                bw.Write("Файл успешно перемещён");
            }
            catch (Exception E) { bw.Write($"Не удалось переместить файл - {E.Message}"); }
            cs.Send(ms_buf);
        }

        void HandleRename()
        {
            string path = br.ReadString();
            path = "ServerData/" + path;
            FileInfo fi = new FileInfo(path);
            try
            {
                fi.MoveTo($"{(fi.DirectoryName == "ServerData" ? "" : fi.DirectoryName)}/{br.ReadString()}");
                bw.Write("Файл успешно переименован");
            }
            catch (Exception E) { bw.Write($"Не удалось переименовать файл - {E.Message}"); }
            cs.Send(ms_buf);
        }

    }
}

