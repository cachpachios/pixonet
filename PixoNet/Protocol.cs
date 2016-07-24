namespace PixoNet
{
    abstract class Protocol
    {
        public abstract Packet CreateInstance(int id);
    }
}
