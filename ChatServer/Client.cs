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
      
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //Сокет этого экземпляра 
        static List<Socket> socketsList = new List<Socket>(); // Список всех созданных сокетов
        static Stack<string> messages = new Stack<string>(); // Список всех сообщений
        static List<String> namesOfClients = new List<string>(); // Список всех клиентов всех сокетов
        string nameOfClient; // Имя клиента данного сокета

        /// <summary>
        /// Добавляет имя клиента в список имен. Если такое имя уже есть, то добавляет номер.
        /// Отправляет всем подключеным клиентом обновленый список клиентов.
        /// </summary>
        /// <param name="client">Сокет клиента</param>
        /// <param name="name">Имя клиента</param>
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

        /// <summary>
        /// Пока есть подключение по сокету ожидает сообщения от клиента.
        /// Поле получения сообщения добавляет его в общий список сообщений и отправляет всем подключеным клиентам.
        /// При получении "-1" отключает клиента, убирает его из списка клиентов и сокетов, отправляет подключеным клиентам новый список клиентов.
        /// </summary>
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

        /// <summary>
        /// Отправляет последнее сообщение клиентам с указанием автора
        /// </summary>
        /// <param name="name">Автор сообщения</param>
        static void SendMessageToClient(string name)
        {
            foreach (Socket s in socketsList)
            {
                if (s.Connected)
                    s.Send(Encoding.UTF8.GetBytes(name + ": " + messages.Peek()));
            }
            
        }

        /// <summary>
        /// Отправляет список клиентов
        /// </summary>
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
