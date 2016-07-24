namespace PixoNet
{
    public abstract class Protocol
    {
        public abstract Packet CreateInstance(int id);
    }
}
