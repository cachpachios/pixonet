using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PixoNet
{
    public class ByteList //TODO: Not use an arraylist :I
    {

        byte[] buffer;
        int bufferPos;
        int increment;

        public ByteList(int initialSize, int expandAmount)
        {
            this.buffer = new byte[initialSize];
            this.increment = expandAmount;
            this.bufferPos = 0;
        }

        public void Write(byte[] buf)
        {
            Write(buf, 0, buf.Length);
        }

        public void Write(byte[] buf, int offset, int count)
        {
            if (buffer.Length - bufferPos >= count)
            {
                Buffer.BlockCopy(buf, offset, buffer, bufferPos, count);
                bufferPos += buf.Length;
            }
            else
            {
                byte[] newBuffer = new byte[buffer.Length + buf.Length + increment];
                Buffer.BlockCopy(buffer, 0, newBuffer, 0, bufferPos);
                Buffer.BlockCopy(buf, offset, newBuffer, bufferPos, count);
                buffer = newBuffer;
            }
        }

        private void EnsureBuffer(int count)
        {
            if (buffer.Length - bufferPos >= count) return;
            byte[] newBuffer = new byte[buffer.Length + increment + count];
            Buffer.BlockCopy(buffer, 0, newBuffer, 0, bufferPos);
            buffer = newBuffer;
        }
        
        private void WriteByteUnsafe(byte b)
        {
            buffer[bufferPos++] = b;
        }

        public void Write(short value)
        {
            EnsureBuffer(2);
            WriteByteUnsafe((byte)(0xff & value >> 8));
            WriteByteUnsafe((byte)(0xff & value));
        }

        public void Write(int value)
        {
            EnsureBuffer(4);
            WriteByteUnsafe((byte)(0xff & value >> 24));
            WriteByteUnsafe((byte)(0xff & value >> 16));
            WriteByteUnsafe((byte)(0xff & value >> 8));
            WriteByteUnsafe((byte)(0xff & value));
        }

        public void Write(long value)
        {
            EnsureBuffer(8);
            WriteByteUnsafe((byte)(0xff & value >> 56));
            WriteByteUnsafe((byte)(0xff & value >> 48));
            WriteByteUnsafe((byte)(0xff & value >> 40));
            WriteByteUnsafe((byte)(0xff & value >> 32));
            WriteByteUnsafe((byte)(0xff & value >> 24));
            WriteByteUnsafe((byte)(0xff & value >> 16));
            WriteByteUnsafe((byte)(0xff & value >> 8));
            WriteByteUnsafe((byte)(0xff & value));
        }

        public void Write(string str)
        {
            EnsureBuffer(str.Length + 2);
            ushort value = (ushort)str.Length;
            WriteByteUnsafe((byte)(0xff & value >> 8));
            WriteByteUnsafe((byte)(0xff & value));
            Write(Encoding.UTF8.GetBytes(str));
        }

        public unsafe void Write(float value)
        {
            Write(*(int*)(&value));
        }

        public unsafe void Write(double value)
        {
            Write(*(long*)(&value));
        }

        public int GetLength()
        {
            return bufferPos;
        }

        public byte[] ToArray()
        {
            byte[] buf = new byte[bufferPos];
            Buffer.BlockCopy(buffer, 0, buf, 0, bufferPos);
            return buf;
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
            Buffer.BlockCopy(buffer, 0, arr, 4, bufferPos + 4);
            return arr;
        }

        public int GetBufferPosition()
        {
            return bufferPos;
        }

        public void SkipBytes(int count)
        {
            EnsureBuffer(count);
            bufferPos += count;
        }

        public void ToBufferPos(int pos)
        {
            bufferPos = pos;
        }

        public void WriteToStream(Stream s)
        {
            s.Write(buffer, 0, bufferPos);
        }
    }
}