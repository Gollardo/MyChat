using Json.Net;
using MyChatClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Stack<string> messages = new Stack<string>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowModel();
        }

        public void ConnectServer()
        {
            byte[] bufferOut = new byte[1024];
            byte[] bufferIn = new byte[1024];

            try
            {
                socket.Connect("127.0.0.1", 904);
                socket.Send(Encoding.UTF8.GetBytes(userNameTextBox.Text));
                socket.Receive(bufferIn);
                string m = Encoding.UTF8.GetString(bufferIn).Trim('\0');
                if (m.StartsWith("["))
                {
                    updateClientsList(bufferIn);
                }


                Thread t = new Thread(new ThreadStart(ListenServer));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
            catch (Exception ex)
            {
                onlineTextBlock.Text = "Подключение не удалось";
                buttonConnect.IsEnabled = true;
                userNameTextBox.IsReadOnly = false;
                buttonDisconnect.IsEnabled = false;
            }

        }

        public void SendMessage(string message)
        {
            byte[] bufferOut = new byte[1024];

            bufferOut = Encoding.UTF8.GetBytes(message);
            if (socket.Connected)
                socket.Send(bufferOut);
        }

        public void DisconnectServer()
        {
            if (socket.Connected)
            {
                socket.Send(Encoding.UTF8.GetBytes("-1"));
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
           
               
            }

        }

        public void ListenServer()
        {
            do
            {
                byte[] bufferIn = new byte[1024];
                socket.Receive(bufferIn);
                string m = Encoding.UTF8.GetString(bufferIn).Trim('\0');
                if (m.StartsWith("["))
                {
                    updateClientsList(bufferIn);
                }
                else
                {
                    messages.Push(m);

                    Dispatcher.BeginInvoke(new ThreadStart(delegate {
                        Label label = new Label();
                        label.Content = messages.Peek();
                        //messagesStack.Children.Add(label);
                    }));

                }

            } while (socket.Connected);

        }

        void updateClientsList(byte[] bufferIn)
        {
            //string m = Encoding.UTF8.GetString(bufferIn).Trim('\0');
            //m = m.Trim(new char[] { '[', ']', '"', ' '});
            //List<string> list = m.Split(',').ToList<string>();


            //Dispatcher.BeginInvoke(new ThreadStart(delegate {
            //    clientsStack.Children.Clear();
            //    foreach (string s in list)
            //    {
            //        TextBlock textBlock = new TextBlock();
            //        textBlock.Text = s.Trim(new char[] { '[', ']', '"', '\''}); ;
            //        clientsStack.Children.Add(textBlock);
            //    }
            //}));
        }

    
        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            buttonConnect.IsEnabled = false;
            userNameTextBox.IsReadOnly = true;
            buttonDisconnect.IsEnabled = true;
            onlineTextBlock.Text = "";
            ConnectServer();
           
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage(messageTextBox.Text);
            messageTextBox.Text = "";
        }

        private void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            buttonConnect.IsEnabled = true;
            userNameTextBox.IsReadOnly = false;
            buttonDisconnect.IsEnabled = false;
            DisconnectServer();
            
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                messageTextBox.AppendText("");
                buttonSend.Command.Execute("123");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (socket.Connected)
            {
                socket.Send(Encoding.UTF8.GetBytes("-1"));
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                finally
                {
                    socket.Close();
                }
            }
            
        }

        private void ListBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
