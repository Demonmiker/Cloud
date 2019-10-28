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
            FileInfo FI = null;
            DirectoryInfo DI = null;
            String path = br.ReadString();
            path = "ServerData/" + path;
            if (path.Contains('.')) FI = new FileInfo(path);
            else DI = new DirectoryInfo(path);
            if (FI != null) try
                {
                    FI.Delete();
                    bw.Write("Файл успешно удалён");
                }
                catch (Exception E) { bw.Write($"Не удалось переместить файл - {E.Message}"); }
            else try
                {
                    DI.Delete();
                    bw.Write("Директория успешно удалена");
                }
                catch (Exception E) { bw.Write($"Не удалось переместить директорию - {E.Message}"); }
            cs.Send(ms_buf);
        }

        void HandleMove()
        {
            FileInfo FI = null;
            DirectoryInfo DI = null;
            String path = br.ReadString();
            path = "ServerData/" + path;
            if (path.Contains('.')) FI = new FileInfo(path);
            else DI = new DirectoryInfo(path);
            if (FI != null) try
                {
                    FI.MoveTo($"{br.ReadString()}/{FI.Name}");
                    bw.Write("Файл успешно перемещён");
                }
                catch (Exception E) { bw.Write($"Не удалось переместить файл - {E.Message}"); }
            else try
                {
                    DI.MoveTo($"{br.ReadString()}/{DI.Name}");
                    bw.Write("Директория успешно перемещена");
                }
                catch (Exception E) { bw.Write($"Не удалось переместить директорию - {E.Message}"); }
            cs.Send(ms_buf);
        }

        void HandleRename()
        {
            FileInfo FI = null;
            DirectoryInfo DI = null;
            String path = br.ReadString();
            path = "ServerData/" + path;
            if (path.Contains('.')) FI = new FileInfo(path);
            else DI = new DirectoryInfo(path);
            if (FI != null) try
                {
                    FI.MoveTo($"{(FI.DirectoryName == "ServerData" ? "" : FI.DirectoryName)}/{br.ReadString()}");
                    bw.Write("Файл успешно переименован");
                }
                catch (Exception E) { bw.Write($"Не удалось переименовать файл - {E.Message}"); }
            else try
                {
                    DI.MoveTo($"{DI.Parent.Name}/{br.ReadString()}");
                    bw.Write("Директория успешно переименована");
                }
                catch (Exception E) { bw.Write($"Не удалось переименовать директорию - {E.Message}"); }
            cs.Send(ms_buf);
        }

    }
}

