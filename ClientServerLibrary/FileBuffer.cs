using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ClientServerLibrary
{
    public class NetBuffer
    {
        MemoryStream Ms;
        public byte[] Ms_Buf;
        public BinaryReader Br;
        public BinaryWriter Bw;
        long size;

        public NetBuffer()
        {
            Init(1);
        }

        public void Init(long _size)
        {
            size = _size;
            Ms_Buf = new byte[size];
            Ms = new MemoryStream(Ms_Buf);
            Br = new BinaryReader(Ms);
            Bw = new BinaryWriter(Ms);
        }

        public void Clear()
        {
            Ms.SetLength(0);
            Ms.SetLength(size);
        }
    }
}
