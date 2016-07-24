﻿using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace PixoNet
{
    class Client
    {
        private TcpClient client;
        private Thread thread;

        private BinaryWriter writer;
        private BinaryReader reader;

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

        public void Establish()
        {
            this.client = new TcpClient(ADDRESS, PORT);
            thread = new Thread(ThreadRun);
            thread.Start();
        }

        private void ThreadRun()
        {
            while(isActive)
            {
                int nextByte = client.GetStream().ReadByte();
                if (nextByte == -1)
                {
                    this.isActive = false;
                    break;
                }
                else
                {
                    protocol.CreateInstance(nextByte);
                }
                Thread.Sleep(15);
            }
        }

        public void Write(Packet packet)
        {
            writer.Write((byte) packet.getID());
            packet.Write(writer);
        }

        public void Flush()
        {
            client.GetStream().Flush();
        }

        public void Close()
        {
            this.isActive = false;
            this.client.GetStream().Close();
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
    }
}