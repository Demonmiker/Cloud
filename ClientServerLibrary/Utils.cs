﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ClientServerPrototype
{
    public static class Utils
    {
        public static bool SaveFile(BinaryReader br,string path)
        {
            string Name = br.ReadString();
            int Length = (int)br.ReadInt64();
            Byte[] buf = br.ReadBytes(Length);
            FileStream FS = new FileStream(path+"/"+Name, FileMode.Create);
            FS.Write(buf, 0, Length);
            Console.WriteLine("Файл успешно сохранён");
            FS.Close();
            return true;
        }

        public static bool LoadFile(BinaryWriter bw,string path)
        {
            FileStream FS = new FileStream(path, FileMode.Open);
            FileInfo FI = new FileInfo(path);
            Byte[] buf = new Byte[FI.Length];
            FS.Read(buf, 0, buf.Length);
            bw.Write(FI.Name);
            bw.Write((Int64)FI.Length);
            bw.Write(buf);
            FS.Close();
            return true;
        }
    }
}