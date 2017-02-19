using System;
using System.Collections.Generic;
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

        private Queue<Packet> inboundQueue;
        private Object queueLock;

        public readonly string ADDRESS;
        public readonly int PORT;

        private volatile bool isActive;

        public readonly Protocol protocol;

        public Client(Protocol protocol, string ip, int port)
        {
            queueLock = new object();
            this.protocol = protocol;
            this.ADDRESS = ip;
            this.PORT = port;

            this.inboundQueue = new Queue<Packet>();
        }

        public void Establish(bool useBigEndian)
        {
            this.client = new TcpClient(ADDRESS, PORT);
            this.client.NoDelay = true;
            thread = new Thread(ThreadRun);
            thread.Start();
        }

        private void ThreadRun()
        {
            try
            {
                this.isActive = true;
                while (isActive)
                {
                    int nextByte = client.GetStream().ReadByte();
                    if (nextByte == -1)
                    {
                        this.isActive = false;
                        return;
                    }
                    else
                    {
                        byte[] lBuf = new byte[4];
                        lBuf[0] = (byte)(nextByte & 0xFF);
                        client.GetStream().Read(lBuf, 1, 3);
                        int l = BitConverter.ToInt32(ensureBigEndian(lBuf), 0);
                        byte[] bBuf = new byte[l];
                        int readbytes = 0;
                        while (readbytes < l)
                        {
                            int b = client.GetStream().ReadByte();
                            if (b == -1)
                            {
                                this.isActive = false;
                                return;
                            }
                            bBuf[readbytes++] = (byte)(b & 0xFF);
                            int _amt = 0;
                            if (readbytes - l > 0)
                                _amt += client.GetStream().Read(bBuf, readbytes, l - readbytes);
                            readbytes += _amt;
                        }
                        ByteArray buf = new ByteArray(bBuf);

                        Packet p = protocol.CreateInstance(buf.ReadUInt16());
                        if (p != null)
                        {
                            p.Read(buf);
                            lock (queueLock)
                                inboundQueue.Enqueue(p);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                this.isActive = false;
                Close();
            }
        }

        public void Write(Packet packet)
        {
            if (!isActive) return;
            ByteList buf = new ByteList(packet.expectedWriteSize() + 8);
            buf.Write(packet.getID());
            packet.Write(buf);
            byte[] a = buf.ToArray(buf.GetLength());
            client.GetStream().Write(a, 0, a.Length);
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
            return inboundQueue.Count > 0;
        }

        public Packet PollPacket()
        {
            Packet p;
            lock (queueLock)
                p = inboundQueue.Dequeue();
            return p;
        }

        private byte[] ensureBigEndian(byte[] input)
        {
            if (BitConverter.IsLittleEndian)
                return input.Reverse().ToArray();
            else return input;
        }

        public TcpClient GetUnderlyingTCPClient()
        {
            return client;
        }
    }
}