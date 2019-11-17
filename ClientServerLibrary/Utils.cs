using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ClientServerLibrary
{
    public static class Utils
    {
        public static Boolean SaveFile(BinaryReader br,String path)
        {
            try
            {
                String Name = br.ReadString();
                int Length = (int)br.ReadInt64();
                Byte[] buf = br.ReadBytes(Length);
                FileStream FS = new FileStream(path + "/" + Name, FileMode.Create);
                FS.Write(buf, 0, Length);
                //Console.WriteLine("Файл успешно сохранён");
                FS.Close();
            }
            catch { return false; }
            return true;
        }

        public static Boolean LoadFile(BinaryWriter bw, String path)
        {
            try
            {
                if (!File.Exists(path))
                    return false;
                FileStream FS = new FileStream(path, FileMode.Open);
                FileInfo FI = new FileInfo(path);
                Byte[] buf = new Byte[FI.Length];
                FS.Read(buf, 0, buf.Length);
                bw.Write(FI.Name);
                bw.Write((Int64)FI.Length);
                bw.Write(buf);
                FS.Close();
            }
            catch(Exception E) { return false; }
            return true;
        }

        public static Boolean LoadFile(BinaryWriter bw, String path, int Position)
        {
            try
            {
                if (!File.Exists(path))
                    return false;
                FileStream FS = new FileStream(path, FileMode.Open);
                FileInfo FI = new FileInfo(path);
                Byte[] buf = new Byte[FI.Length];
                FS.Read(buf, 0, buf.Length);
                bw.Write(FI.Name);
                bw.Write((Int64)FI.Length);
                //
                Byte[] bufD = new Byte[FI.Length - Position];
                buf.CopyTo(bufD, Position);
                //
                bw.Write(bufD);
                FS.Close();
            }
            catch (Exception E) { return false; }
            return true;
        }


        public static Boolean CheckFile(String path)
        {
            try { FileStream FS = new FileStream(path, FileMode.Open);FS.Close(); }
            catch {  return false; }
            return true;
        }

        public static long FileSize(String path)
        {
            try
            {
                return new FileInfo(path).Length;
            }
            catch
            {
                return -1L;
            }
        }


    }
}
