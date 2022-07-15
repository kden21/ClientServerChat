using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfClient
{
    internal class Chat
    {
        public delegate void ShowMessage(Message m);
        ShowMessage showMessage;

        public int Port {get; set;}
        public string Address { get; set; }
        Socket? socket;
        public Chat(ShowMessage showMessage)
        {
            this.showMessage = showMessage;
        }

        public void ConnectSocket()
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(Address), Port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);

            Task.Run(async () =>
            {
                while (true)
                {
                    string? mes = await ReceiveMes();
                    List<Message>? messages = JsonSerializer.Deserialize<List<Message>>(mes!);
                    var sortedMessages=from m in messages
                             orderby m.Id
                             select m;
                    foreach(Message m in sortedMessages)
                        showMessage(m);
                }
            });
        }

        private async Task<string?> ReceiveMes()
        {
            return await Task<string?>.Run(() => {
                byte[] data = new byte[256]; 
                StringBuilder builder = new StringBuilder();
                int bytes = 0; 
                try
                {
                    do
                    {
                        bytes = socket!.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);
                    return builder.ToString();
                }
                catch (System.Net.Sockets.SocketException)
                {
                    return null;
                }                    
            });     
        }

        public async Task SendMes(Message msg)
        {
            await Task.Run(() => 
            {
                string json = JsonSerializer.Serialize(msg);
                byte[] data = Encoding.Unicode.GetBytes(json);
                socket!.Send(data);
            });           
        }

        public void CloseSocket()
        {
            socket!.Close();
        }
    }
}
