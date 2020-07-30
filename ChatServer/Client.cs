using Json.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ChatServer
{
    public class Client
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static List<Socket> socketsList = new List<Socket>();
        static Stack<string> messages = new Stack<string>();
        static List<String> namesOfClients = new List<string>();
        string nameOfClient;

        public Client(Socket client, string name)
        {
            int indexName = 1;
            string foo = name.Trim('\0');

            socket = client;
            socketsList.Add(socket);

            while (true)
            {
                
                if (namesOfClients.Contains(foo))
                {
                    foo = name.Trim('\0') + indexName;
                }
                else
                {
                    break;
                }
                indexName++;
            }

            nameOfClient = foo;
            namesOfClients.Add(foo);

            SendListOfClientsToClient();
        }

       public void WorkWhithClient()
        {
            do
            {
                byte[] bufferIn = new byte[1024];
                socket.Receive(bufferIn);
                string m = Encoding.UTF8.GetString(bufferIn).Trim('\0');
                if( m == "-1")
                {
                    socketsList.Remove(socket);
                    namesOfClients.Remove(nameOfClient);
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    SendListOfClientsToClient();
                }
                else
                {
                    messages.Push(m);
                    Console.WriteLine("Сообщение от " + nameOfClient + ": " + m);
                    SendMessageToClient(nameOfClient);
                }

            } while (socket.Connected);
        }

        static void SendMessageToClient(string name)
        {
            foreach (Socket s in socketsList)
            {
                if (s.Connected)
                    s.Send(Encoding.UTF8.GetBytes(name + ": " + messages.Peek()));
            }
            
        }

        static void SendListOfClientsToClient()
        {
            string json = JsonNet.Serialize(namesOfClients);
            

            foreach (Socket s in socketsList)
            {
                if (s.Connected)
                {
                    s.Send(Encoding.UTF8.GetBytes(json));
                }

            }

        }
    }
}
