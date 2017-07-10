using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PixoNet
{
    public class ByteArray
    {
        private byte[] buffer;
        private int position;

        public ByteArray(byte[] buffer)
        {
            this.buffer = buffer;
            position = 0;
        }

        public byte ReadByte()
        {
            return buffer[position++];
        }

        public short ReadInt16()
        {
            return BitConverter.ToInt16(ensureBigEndian(ReadBytes(2)), 0);
        }

        public int ReadInt32()
        {
            return BitConverter.ToInt32(ensureBigEndian(ReadBytes(4)), 0);
        }

        public long ReadInt64()
        {
            return BitConverter.ToInt64(ensureBigEndian(ReadBytes(8)), 0);
        }

        public ushort ReadUInt16()
        {
            return BitConverter.ToUInt16(ensureBigEndian(ReadBytes(2)), 0);
        }

        public uint ReadUInt32()
        {
            return BitConverter.ToUInt32(ensureBigEndian(ReadBytes(4)), 0);
        }

        public ulong ReadUInt64()
        {
            return BitConverter.ToUInt64(ensureBigEndian(ReadBytes(8)), 0);
        }

        public unsafe float ReadFloat()
        {
            int v = ReadInt32();
            return *(float*)(&v);
        }

        public byte[] ReadBytes(int amount)
        {
            byte[] data = new byte[amount];
            Array.Copy(buffer, position, data, 0, amount);
            position += amount;
            return data;
        }

        public void ReadBytes(byte[] destination, int amount)
        {
            Array.Copy(buffer, position, destination, 0, amount);
            position += amount;
        }

        public void ReadBytes(byte[] destination, int destinationIndex, int amount)
        {
            Array.Copy(buffer, position, destination, destinationIndex, amount);
            position += amount;
        }

        public string ReadString()
        {
            return Encoding.UTF8.GetString(ReadBytes(ReadUInt16()));
        }

        public string ReadShortString()
        {
            return Encoding.UTF8.GetString(ReadBytes(ReadByte()));
        }

        public void SkipBytes(int amount)
        {
            position += amount;
        }

        public int GetBytesRemaining()
        {
            return buffer.Length - position;
        }

        private byte[] ensureBigEndian(byte[] input)
        {
            if (BitConverter.IsLittleEndian)
                return input.Reverse().ToArray();
            else return input;
        }

    }
}