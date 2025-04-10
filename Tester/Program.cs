﻿using PixoNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            Client client = new Client(new TestProtocol(), "localhost", 7331);
            client.Establish(true);

            IntroducePacket intro = new IntroducePacket();
            intro.protocol = 12345;
            intro.clientID = 123;
            intro.authID = 2378123723;

            client.Write(intro);
            client.Flush();

            while (true)
            {
                if (client.alive() == false)
                {
                    Console.WriteLine("Closed");
                    break;
                }
                Thread.Sleep(50);
                if (client.hasPacket())
                {
                    Packet p = client.PollPacket();
                    Console.WriteLine(p.getID());
                    if (p.getID() == 0)
                        Console.WriteLine("Welcome " + ((WelcomePacket)p).status);
                }
            }
            client.Close();

            while (true) Thread.Sleep(10);
        }
    }
}
