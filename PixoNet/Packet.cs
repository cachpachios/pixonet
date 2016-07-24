using System.IO;

namespace PixoNet
{
    public abstract class Packet
    {
        public abstract int getID();

        public virtual void Read(BinaryReader reader) { }
        public virtual void Write(BinaryWriter writer) { }
    }
}