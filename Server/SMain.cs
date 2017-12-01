using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerData;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;


namespace Server
{
    class Server
    {

        static Socket listenerSocket;
        static List<ClientData> _clients;











        static void Main(string[] args)
        {

            Console.WriteLine("Started server on " + Packet.GetIPv4Address());
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clients = new List<ClientData>();

            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(Packet.GetIPv4Address()), 4242);
            listenerSocket.Bind(ip);

            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();





        } // start server







        static void ListenThread() // This thread listens for incomming connections, and adds them to the list of clients.
        {
            for (;;)
            {

                listenerSocket.Listen(0);
                _clients.Add(new ClientData(listenerSocket.Accept()));

            } // Maybe fix this seems CPU intensive.


        }

        //clientdata thread - receives data from each client individually
        public static void Data_IN(object cSocket)
        {
            Socket clientSocket = (Socket)cSocket;

            byte[] Buffer;
            int readBytes;

            for (;;)
            {
                Buffer = new byte[clientSocket.SendBufferSize];

                readBytes = clientSocket.Receive(Buffer);

                if (readBytes > 0)
                {
                    Packet packet = new Packet(Buffer);
                    DataManager(packet); // Pass the packet to the manager.

                }



            }


        }

        //data manager 
        public static void DataManager(Packet p)
        {
            switch (p.packetType)
            {
                case Packet.PacketType.Registration:
                    break;
                case Packet.PacketType.Chat:

                    foreach (ClientData c in _clients)
                    {
                        c.clientSocket.Send(p.ToBytes());
                    }

                    break;
                default:
                    break;
            }
        }


    }




    class ClientData
    {

        public Socket clientSocket;
        public Thread clientThread; //
        public string id;



        public ClientData()
        {
            id = Guid.NewGuid().ToString(); // Generates a unique random number to identify the client.
            clientThread = new Thread(Server.Data_IN);
            clientThread.Start(clientSocket);
            SendRegistrationPacket();
        }

        public ClientData(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(Server.Data_IN);
            clientThread.Start(clientSocket);
            SendRegistrationPacket();
        }

        public void SendRegistrationPacket()
        {
            Packet p = new Packet(Packet.PacketType.Registration, "server");
            p.Gdata.Add(id);
            clientSocket.Send(p.ToBytes());

        }


    }










}
