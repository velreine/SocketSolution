﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerData;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Client
{
    class Client
    {


        public static Socket master;
        public static string name;
        public static string id;









        static void Main(string[] args)
        {

            Console.Write("Enter your name: ");
            name = Console.ReadLine();



        A: Console.Clear();
            Console.Write("Enter host IP address:");
            string ip = Console.ReadLine();

            master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(ip), 4242);


            try
            {
                master.Connect(ipe);
            }
            catch (Exception ex)
            {

                Console.WriteLine("An error occured attempting to connect to the host: " + ex.Message);
                Thread.Sleep(10000);
                goto A;
            }

            Thread t = new Thread(Data_IN);
            t.Start();

            for (;;)
            {
                Console.Write(":#");
                string input = Console.ReadLine();

                Packet p = new Packet(Packet.PacketType.Chat, id);
                p.Gdata.Add(name);
                p.Gdata.Add(input);
                master.Send(p.ToBytes());
            }

        }


        static void Data_IN()
        {

            byte[] Buffer;
            int readBytes;

            for (;;)
            {
                Buffer = new byte[master.SendBufferSize];
                readBytes = master.Receive(Buffer);

                if (readBytes > 0)
                {
                    DataManager(new Packet(Buffer));
                }


            }



        }


        static void DataManager(Packet p)
        {


            switch (p.packetType)
            {
                case Packet.PacketType.Registration:

                    id = p.Gdata[0];


                    break;

                case Packet.PacketType.Chat:
                    ConsoleColor c = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(p.Gdata[0] + ": " + p.Gdata[1]);
                    Console.ForegroundColor = c;

                    break;
                default:
                    break;
            }




        }


    }
}
