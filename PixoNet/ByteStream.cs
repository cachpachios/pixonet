using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace PixoNet
{
    public class ByteStream
    {
        private BufferedStream stream;

        private bool reverseEndian;

        public ByteStream(Stream stream, bool bigEndian)
        {
            this.stream = new BufferedStream(stream);

            if (bigEndian)
                reverseEndian = BitConverter.IsLittleEndian;
            else
                reverseEndian = !BitConverter.IsLittleEndian;
        }

        // Input Operations

        public byte ReadByte()
        {
            return (byte) (stream.ReadByte());
        }
        
        public short ReadInt16()
        {
            byte[] bffr = new byte[2];
            stream.Read(bffr, 0, 2);
            return BitConverter.ToInt16(ensureBigEndian(bffr), 0);
        }

        public int ReadInt32()
        {
            byte[] bffr = new byte[4];
            stream.Read(bffr, 0, 4);
            return BitConverter.ToInt32(ensureBigEndian(bffr), 0);
        }

        public long ReadInt64()
        {
            byte[] bffr = new byte[8];
            stream.Read(bffr, 0, 8);
            return BitConverter.ToInt64(ensureBigEndian(bffr), 0);
        }

        public ushort ReadUInt16()
        {
            byte[] bffr = new byte[2];
            stream.Read(bffr, 0, 2);
            return BitConverter.ToUInt16(ensureBigEndian(bffr), 0);
        }

        public uint ReadUInt32()
        {
            byte[] bffr = new byte[4];
            stream.Read(bffr, 0, 4);
            return BitConverter.ToUInt32(ensureBigEndian(bffr), 0);
        }

        public ulong ReadUInt64()
        {
            byte[] bffr = new byte[8];
            stream.Read(bffr, 0, 8);
            return BitConverter.ToUInt64(ensureBigEndian(bffr), 0);
        }

        public unsafe float ReadFloat()
        {
            int v = ReadInt32();
            return *(float*)(&v);
        }

        public byte[] ReadBytes(int amount)
        {
            byte[] data = new byte[amount];
            stream.Read(data, 0, amount);
            return data;
        }

        // Output Operations
        public void Write(byte v)
        {
            stream.WriteByte(v);
        }

        public void Write(short v)
        {
            stream.Write(ensureBigEndian(BitConverter.GetBytes(v)),0,2);
        }

        public void Write(int v)
        {
            stream.Write(ensureBigEndian(BitConverter.GetBytes(v)), 0, 4);
        }

        public void Write(long v)
        {
            stream.Write(ensureBigEndian(BitConverter.GetBytes(v)), 0, 8);
        }

        public void Write(ushort v)
        {
            stream.Write(ensureBigEndian(BitConverter.GetBytes(v)), 0, 2);
        }

        public void Write(uint v)
        {
            stream.Write(ensureBigEndian(BitConverter.GetBytes(v)), 0, 4);
        }

        public void Write(ulong v)
        {
            stream.Write(ensureBigEndian(BitConverter.GetBytes(v)), 0, 8);
        }

        public unsafe void Write(float v)
        {
            Write(*(int*)(&v));
        }

        public void Flush()
        {
            stream.Flush();
        }

        public void Close()
        {
            stream.Close();
        }

        private byte[] ensureBigEndian(byte[] input) {
            if (reverseEndian)
                return input.Reverse().ToArray();
            else return input;
        }
    }
}