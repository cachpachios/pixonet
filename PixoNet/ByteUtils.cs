using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PixoNet
{
    class ByteUtils
    {
        public static byte[] WriteShort(short v)
        {
            return new byte[] {(byte)(0xff & v >> 8), (byte)(0xff & v)};
        }

        public static byte[] WriteByte(byte v)
        {
            return new byte[] { v };
        }

        public static byte[] WriteInt(int intValue)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(0xff & intValue >> 24);
            bytes[1] = (byte)(0xff & intValue >> 16);
            bytes[2] = (byte)(0xff & intValue >> 8);
            bytes[3] = (byte)(0xff & intValue);
            return bytes;
        }

        public unsafe byte[] WriteFloat(float v)
        {
            return WriteInt(*(int*)(&v));
        }

        public static byte[] Reverse(byte[] array)
        {
            return array.Reverse().ToArray();
        }
    }
}
