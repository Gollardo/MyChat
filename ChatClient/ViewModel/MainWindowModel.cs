﻿using ChatClient;
using MyChatClient.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyChatClient.ViewModel
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;  //Нужно для уведомления об изменении параметра
        private static List<string> messages = new List<string>(); // Список сообщений
        private Server server = new Server(); // Сервер
        Thread t; // Поток для прослушивания сервера

        // Сообщение от клиента
        private string clientMessage = "";

        /// <summary>
        /// Список клиентов. Реализован через ObservableCollection<string>, потому что требуется 
        /// реализация INotifyPropertyChanged внутри колекции.
        /// </summary>
        public ObservableCollection<string> Clients { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Список сообщений. Реализован через ObservableCollection<string>, потому что требуется 
        /// реализация INotifyPropertyChanged внутри колекции.
        /// </summary>
        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

        public string ClientMessage
        {
            get
            {
                return clientMessage;
            }
            set
            {
                clientMessage = value;
                OnPropertyChanged("ClientMessage");
            }
        }

        // Команда на подключение к серверу
        public ConnectToServerCommand ConnectCommand { get; set; }
        
        //Команда на отключение от сервера
        public DisconectFromServerCommand DisconectCommand { get; set; }

        public SendMessageCommand SendCommand { get; set; }

        public MainWindowModel()
        {
            this.ConnectCommand = new ConnectToServerCommand(this);
            this.DisconectCommand = new DisconectFromServerCommand(this);
            this.SendCommand = new SendMessageCommand(this);

           
        }

        //Уведомляет об изменении параметра
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Подключается к серверу и запускает прослушивание сервера в отдельном потоке
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        public void ConnectToServer(string userName)
        {
            server.ConnectServer(userName);
            t = new Thread(new ThreadStart(ListenServer));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        /// <summary>
        /// Отправляет сообщение на сервер
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        public void SendMessageToServer()
        {
            server.SendMessage(ClientMessage);
            ClientMessage = "";
        }

        /// <summary>
        /// Отключается от сервера
        /// </summary>
        public void DisconectFromServer()
        {
            server.IsConnectedToServer = false;
            t.Abort();
            server.DisconnectServer();
        }

        /// <summary>
        /// Пока есть подключение к серверу ожидает сообщения от сервера
        /// </summary>
        public void ListenServer()
        {
            string message = "";

            do
            {
               if(server.IsConnectedToServer)
                message = server.ListenServer();

                if (message.StartsWith("["))
                {
                    App.Current.Dispatcher.Invoke((Action)delegate { Clients.Clear();});
                    message = message.Trim(new char[] { '[', ']', '"', '\'' });
                    List<string> list = message.Split(',').ToList<string>();
                    foreach (string s in list)
                    {
                        App.Current.Dispatcher.Invoke((Action)delegate { Clients.Add(s); });
                    }
                    
                }
                else if (!message.Equals(""))
                {
                    App.Current.Dispatcher.Invoke((Action)delegate { Messages.Add(message); });
                }
            } while (server.IsConnectedToServer);
        }
       


    }
}
