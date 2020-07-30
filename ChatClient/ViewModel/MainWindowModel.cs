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

        /// <summary>
        /// Возвращает список сообщений
        /// </summary>
        //public List<string> Messages
        //{
        //    get
        //    {
        //        return messages;
        //    }
        //}

        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

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
            Thread t = new Thread(new ThreadStart(ListenServer));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        /// <summary>
        /// Отправляет сообщение на сервер
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        public void SendMessageToServer(string message)
        {
            server.SendMessage(message);
        }

        /// <summary>
        /// Отключается от сервера
        /// </summary>
        public void DisconectFromServer()
        {
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

                message = server.ListenServer();

                if (message.StartsWith("["))
                {
                    // TODO: сделать обновление списка пользователей
                }
                else
                {
                    Messages.Add(message);
                    //OnPropertyChanged("Messages");
                }
            } while (server.IsConnectedToServer);
        }
       


    }
}
