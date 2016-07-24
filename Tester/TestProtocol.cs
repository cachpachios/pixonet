using PixoNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    class TestProtocol : Protocol
    {
        public override Packet CreateInstance(int id)
        {
            if (id == 0)
                return new WelcomePacket();

            return null; 
        }
    }

    public class IntroducePacket : Packet
    {
        public ushort protocol;
        public int clientID;
        public long authID;

        public override int getID()
        {
            return 0;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(protocol);
            writer.Write(clientID);
            writer.Write(authID);
            Console.WriteLine("Introduce!");
        }
    }

    public class WelcomePacket : Packet
    {
        public byte status;

        public override int getID()
        {
            return 0;
        }

        public override void Read(BinaryReader reader)
        {
            status = reader.ReadByte();
        }
    }
}
