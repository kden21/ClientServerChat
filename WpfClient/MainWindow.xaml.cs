using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace WpfClient
{
    public partial class MainWindow : Window
    {
        bool isConnected = false;
        Chat chat;       
        public MainWindow()
        {
            InitializeComponent();
            chat = new Chat(ShowMessage);
            btSend.IsEnabled = false;
        }

        void ConnectUser() 
        { 
            if(!isConnected)
            {
                lbChat.Items.Clear();
                btSend.IsEnabled = true;
                chat.Port = int.Parse(tbPort.Text);
                chat.Address = tbAddress.Text;
                chat.ConnectSocket();
                tbUserName.IsEnabled = false;
                tbPort.IsEnabled = false;
                tbAddress.IsEnabled = false;
                bConDoscon.Content = "Отключиться";
                isConnected = true;            
            }
        }

        void DisconnectUser()
        {
            if (isConnected)
            {
                btSend.IsEnabled = false;
                tbUserName.IsEnabled = true;
                tbPort.IsEnabled = true;
                tbAddress.IsEnabled = true;               
                bConDoscon.Content = "Подключится";
                isConnected = false;
                chat.CloseSocket();
            }
        }

        void Button_Click(object sender,RoutedEventArgs e)
        {
            if (isConnected)
                DisconnectUser();
            else
                ConnectUser();
        }

        public void ShowMessage(Message message)
        {
            Dispatcher.BeginInvoke((Action)(() => lbChat.Items.Add(message)));           
        }
        
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Message msg = new Message { Time = DateTime.Now.ToString("G"), Text = tbMessage.Text , UserName = tbUserName.Text };
            await chat.SendMes(msg);
            lbChat.Items.Add(msg);
            tbMessage.Clear();
        }
    }
}
