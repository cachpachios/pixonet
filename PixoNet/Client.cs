using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace PixoNet
{
    public class Client
    {
        private TcpClient client;
        private Thread thread;

        private ConcurrentQueue<Packet> inboundQueue;

        public readonly string ADDRESS;
        public readonly int PORT;

        private volatile bool isActive;

        public readonly Protocol protocol;

        public Client(Protocol protocol, string ip, int port)
        {
            this.protocol = protocol;
            this.ADDRESS = ip;
            this.PORT = port;

            this.inboundQueue = new ConcurrentQueue<Packet>();
        }

        public void Establish(bool useBigEndian)
        {
            this.client = new TcpClient(ADDRESS, PORT);
            thread = new Thread(ThreadRun);
            thread.Start();
        }

        private void ThreadRun()
        {
            isActive = true;
            while(isActive)
            {
                Thread.Sleep(5);
                if (!client.GetStream().DataAvailable) continue;

                int nextByte = client.GetStream().ReadByte();
                if (nextByte == -1)
                {
                    this.isActive = false;
                    break;
                }
                else
                {
                    byte[] lBuf = new byte[4];
                    lBuf[0] = (byte) (nextByte & 0xFF);
                    client.GetStream().Read(lBuf, 1, 3);
                    int l = BitConverter.ToInt32(ensureBigEndian(lBuf),0);
                    Console.Out.WriteLine("Packet Length: " + l);
                    byte[] bBuf = new byte[l];
                    client.GetStream().Read(bBuf, 0, l);
                    ByteArray buf = new ByteArray(bBuf);

                    Packet p = protocol.CreateInstance(buf.ReadUInt16());
                    if(p != null)
                    {
                        p.Read(buf);
                        inboundQueue.Enqueue(p);
                    }
                }
            }
        }

        public void Write(Packet packet)
        {
            ByteList buf = new ByteList(packet.expectedWriteSize() + 16);
            buf.Write(packet.getID());
            packet.Write(buf);

            client.GetStream().Write(ensureBigEndian(BitConverter.GetBytes(buf.GetLength())), 0, 4);
            client.GetStream().Write(buf.ToArray(),0, buf.GetLength());
        }

        public void Flush()
        {
            client.GetStream().Flush();
        }

        public void Close()
        {
            this.isActive = false;
            client.GetStream().Close();
            this.client.Close();
        }

        public bool alive()
        {
            return isActive;
        }

        public bool hasPacket()
        {
            return !inboundQueue.IsEmpty;
        }

        public Packet PollPacket()
        {
            Packet p;
            if (this.inboundQueue.TryDequeue(out p))
                return p;
            return null;
        }

        private byte[] ensureBigEndian(byte[] input)
        {
            if (BitConverter.IsLittleEndian)
                return input.Reverse().ToArray();
            else return input;
        }
    }
}