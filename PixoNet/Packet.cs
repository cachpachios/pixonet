using System.IO;

namespace PixoNet
{
    public abstract class Packet
    {
        public abstract ushort getID();

        public virtual void Read(ByteArray reader) { }
        public virtual void Write(ByteList writer) { }

        public virtual int expectedWriteSize() { return 32; }
    }
}