using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SomeProject.Library.Server
{
    public class Server
    {
        TcpListener serverListener;
        static int usrCount = 0;
        static string root = Environment.CurrentDirectory + @".\" + DateTime.UtcNow.Date.ToString("yyyy-mm-dd");
        static int fileCounter = 0;
        public Server()
        {
            serverListener = new TcpListener(IPAddress.Loopback, 8080);
        }

        /// <summary>
        /// Отключение слушателя TCP
        /// </summary>
        public bool TurnOffListener()
        {
            try
            {
                if (serverListener != null)
                    serverListener.Stop();
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot turn off listener: " + e.Message);
                return false;
            }
        }


        /// <summary>
        /// Включение слушателя TCP
        /// </summary>
        public async void  TurnOnListener()
        {
            try
            {
                Console.WriteLine("Listener started");
                if (serverListener != null)
                    serverListener.Start();

                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }
                while (true)
                {
                    OperationResult result = await RecieveHeaderFromClient();
                    Thread.Sleep(5);//Переключаемся на дркугой Listener
                }

                //while (true)
                //{
                //    OperationResult result = await RecieveHeaderFromClient();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot turn on listener: " + e.Message);
            }
        }

        /// <summary>
        /// Получение закголовка сообщения и выполнение действия с ним
        /// </summary>
        public async Task<OperationResult> RecieveHeaderFromClient()
        {
            try
            {
                if (usrCount > 2) { throw new System.InvalidOperationException("Too many users"); }
                TcpClient client = serverListener.AcceptTcpClient();
                Console.WriteLine();
                Console.WriteLine(DateTime.Now);
                StringBuilder recievedMessage = new StringBuilder();



                byte[] data = new byte[20];
                NetworkStream stream = client.GetStream();
                int bytes = stream.Read(data, 0, data.Length);
                string header = Encoding.UTF8.GetString(data, 0, data.Length);
                if (header.StartsWith("ack@--@:"))
                {
                    Interlocked.Increment(ref usrCount);
                    Console.WriteLine("New Client Added, Total: " + usrCount);
                    while (true) { Thread.Sleep(1000); }//Имитируем общение
                    Interlocked.Decrement(ref usrCount);
                    Console.WriteLine("User left, Total : " + usrCount);
                }
                if (header.StartsWith("msg@--@:"))
                {
                    
                  
                    Console.WriteLine(".Получено сообщение!");
                    data = new byte[1024];
                    int thisRead;
                    do
                    {
                        thisRead = stream.Read(data, 0, data.Length);
                        recievedMessage.Append(Encoding.UTF8.GetString(data, 0, thisRead));
                    }
                    while (stream.DataAvailable);
                    Console.WriteLine("Кто-то говорит :");
                    Console.WriteLine(recievedMessage);
                    
                }
                if (header.StartsWith("file@-"))
                {
                    Console.WriteLine(".Обнаружен файл!");
                    string fileExt = header.Split('-')[1];
                    byte[] fileData = new byte[data.Length - 20];


                    int thisRead;
                    data = new byte[1024];
                    string newFile = root + @"\File_" + (++fileCounter) + fileExt;
                    Stream fileStream = File.OpenWrite(newFile);
                    Console.WriteLine("..Новый файл создан в : " + newFile);
                    do
                    {
                        thisRead = stream.Read(data, 0, data.Length);
                        fileStream.Write(data, 0, thisRead);
                    }
                    while (stream.DataAvailable);
                    fileStream.Close();
                    Console.WriteLine(".Файл успешно добавлен");
                }

             
                return new OperationResult(Result.OK, recievedMessage.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Возникла ошибка :" + e.Message);
                return new OperationResult(Result.Fail, e.Message);
            }

        }
       
    }
}