using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PixoNet
{
    public class ByteList
    {
        private List<byte> buf;
        public ByteList(int expectedSize)
        {
            this.buf = new List<byte>();
        }

        public void Write(byte v)
        {
            buf.Add(v);
        }

        public void Write(short v)
        {
            Write(ensureBigEndian(BitConverter.GetBytes(v)));
        }

        public void Write(int v)
        {
            Write(ensureBigEndian(BitConverter.GetBytes(v)));
        }

        public void Write(long v)
        {
            Write(ensureBigEndian(BitConverter.GetBytes(v)));
        }

        public void Write(ushort v)
        {
            Write(ensureBigEndian(BitConverter.GetBytes(v)));
        }

        public void Write(uint v)
        {
            Write(ensureBigEndian(BitConverter.GetBytes(v)));
        }

        public void Write(ulong v)
        {
            Write(ensureBigEndian(BitConverter.GetBytes(v)));
        }

        public unsafe void Write(float v)
        {
            Write(*(int*)(&v));
        }

        public void Write(byte[] array)
        {
            buf.AddRange(array);
        }

        private byte[] ensureBigEndian(byte[] input)
        {
            if (BitConverter.IsLittleEndian)
                return input.Reverse().ToArray();
            else return input;
        }

        public int GetLength()
        {
            return buf.Count;
        }

        public byte[] ToArray()
        {
            return buf.ToArray();
        }
    }
}
