using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyChatClient
{
    public class Server : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged; //Нужно для уведомления об изменении параметра

        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Сокет

        /// <summary>
        /// Возвращает true если сокет подключен к серверу
        /// </summary>
        public bool IsConnectedToServer = false;

        // Возвращает сокет и записывает значение в сокет с вызовом OnPropertyChanged()
        public Socket MySocket
        {
            get { return socket; }
            set
            {
                socket = value;
                OnPropertyChanged("MySocket");
            }
           
        }

        //Уведомляет об изменении параметра
        private void OnPropertyChanged( string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Подключается к серверу посредством TCP и отправляет имя пользователя.
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <returns>Возвращает bool</returns>
        public bool ConnectServer(string userName)
        {
            if (!socket.Connected)
            {
                byte[] bufferOut = new byte[1024];
                byte[] bufferIn = new byte[1024];

                try
                {
                    socket.Connect("127.0.0.1", 904);
                    socket.Send(Encoding.UTF8.GetBytes(userName));
                    //socket.receive(bufferin);
                    //string m = Encoding.UTF8.GetString(bufferIn).Trim('\0');
                    IsConnectedToServer = true;

                    return true;


                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Отправляет сообщение на сервер в кодировке UTF8
        /// </summary>
        /// <param name="message">Тект сообщения</param>
        public void SendMessage(string message)
        {
            byte[] bufferOut = new byte[1024];

            bufferOut = Encoding.UTF8.GetBytes(message);
            if (socket.Connected)
                socket.Send(bufferOut);
        }

        /// <summary>
        /// Отключается от сервера
        /// </summary>
        /// <returns>Возвращает истину при успешном отключении</returns>
        public bool DisconnectServer()
        {
            if (socket.Connected)
            {
                socket.Send(Encoding.UTF8.GetBytes("-1"));
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    IsConnectedToServer = false;
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }


            }

            return true;

        }

        /// <summary>
        /// Пока есть подключение к серверу, ожидает сообщения
        /// </summary>
        /// <returns>Сообщение от сервера</returns>
        public string ListenServer()
        {
                //byte[] bufferIn = new byte[1024];
                //socket.Receive(bufferIn);
                //string m = Encoding.UTF8.GetString(bufferIn).Trim('\0');
                return "hi";

        }
    }
}
