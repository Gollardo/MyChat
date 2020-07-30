using ChatServer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;

namespace ChasServer
{
    class Program
    {
       
       
        static int clientsCount = 0;
        

        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
          

            socket.Bind(new IPEndPoint(IPAddress.Any, 904));
            socket.Listen(5);

            Console.WriteLine("Сервер запущен");


            while (true)
            {
                byte[] bufferIn = new byte[1024];
                byte[] bufferOut = new byte[1024];

                Socket client = socket.Accept();
                Console.WriteLine("Новое подключение: " + client.RemoteEndPoint.ToString());
                client.Receive(bufferIn);
                Client cl = new Client(client, Encoding.UTF8.GetString(bufferIn));


                Thread clientThread = new Thread(new ThreadStart(cl.WorkWhithClient));
                clientThread.Start();


            }

           
        }
    }
}
