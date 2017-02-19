using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PixoNet
{
    public class ByteList //TODO: Not use an arraylist :I
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

        public byte[] ToArray(int prefix)
        {
            byte[] arr = new byte[GetLength() + 4];
            byte[] prefixBuf = BitConverter.GetBytes(prefix);
            if (BitConverter.IsLittleEndian)
            {
                arr[0] = prefixBuf[3];
                arr[1] = prefixBuf[2];
                arr[2] = prefixBuf[1];
                arr[3] = prefixBuf[0];
            }
            else
            {
                arr[0] = prefixBuf[0];
                arr[1] = prefixBuf[1];
                arr[2] = prefixBuf[2];
                arr[3] = prefixBuf[3];
            }
            Buffer.BlockCopy(ToArray(), 0, arr, 4, GetLength());
            return arr;
        }
    }
}