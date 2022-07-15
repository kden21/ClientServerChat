using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;


namespace ChatServer
{
    class Program
    {
        
        static int port = 8005;
        Test5DBContext context = new Test5DBContext();
        static void Main(string[] args)
        {
            using (var context = new Test5DBContext())
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(10);              
                context.Messages.FirstOrDefault();  //избавляемся от задержки при первом подключении
                List<Socket> sockets = new List<Socket>();              
                try
                {
                    Console.WriteLine("Сервер запущен. Ожидание подключений...");
                    while (true)
                    {                       
                        Socket handler = listenSocket.Accept();
                        sockets.Add(handler);
                        Task.Run(() => {
                            SendMessage(handler, context.Messages.ToList());    
                            while (true)
                            {
                                // получаем сообщение
                                try
                                {
                                    Message? msgRestored = ReceiveMessage(handler, sockets);
                                    if (msgRestored != null)
                                    {
                                        LinkSockets(sockets, handler, msgRestored!);
                                        Console.WriteLine(msgRestored.Time + " " + msgRestored.UserName + ": " + msgRestored.Text);
                                        context.Messages.Add(msgRestored);
                                        context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        Console.WriteLine("The user left the chat :(");
                                        break;
                                    }
                                }
                                catch (System.Text.Json.JsonException)
                                {
                                    handler.Shutdown(SocketShutdown.Both);
                                    Console.WriteLine("The user left the chat :(");
                                    break;
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }           
        }

        public static Message? ReceiveMessage(Socket socket, List<Socket> sockets)
        {
            try
            {
                byte[] data = new byte[256]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт
                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);
                return ConversionfromBytes(builder.ToString());
            }
            catch (System.Net.Sockets.SocketException)
            {
                socket.Close();
                sockets.Remove(socket);
                return null;
            }           
        }

        private static void SendMessage(Socket socket, Message message)
        {
            byte[] data = ConversionToBytes(JsonSerializer.Serialize(new Message[] { message }));
            socket.Send(data);
        }
        private static void SendMessage(Socket socket, List<Message> message)
        {
            byte[] data = ConversionToBytes(JsonSerializer.Serialize(message.ToList()));
            socket.Send(data);
        }

        public static void LinkSockets(List<Socket> sockets, Socket handler, Message mes)
        {
            foreach (var s in sockets)
            {
                if (s != handler)
                {
                    SendMessage(s, mes);
                }
            }
        }

        public static Message? ConversionfromBytes(string mes)
        {
            Message? msgRestored = JsonSerializer.Deserialize<Message>(mes.ToString());           
            return msgRestored;
        }

        public static byte[] ConversionToBytes(string mes)
        {
            byte[] data = Encoding.Unicode.GetBytes(mes);
            return data;
        }
    }
}