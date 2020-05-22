using System;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace SomeProject.Library.Client
{
    public class Client
    {
        public TcpClient tcpClient;

        /// <summary>
        /// Получить сообщение с сервера
        /// </summary>
        public OperationResult ReceiveMessageFromServer()
        {
            try
            {

                StringBuilder recievedMessage = new StringBuilder();
                byte[] data = new byte[256];
                NetworkStream stream = tcpClient.GetStream();

                do
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    recievedMessage.Append(Encoding.UTF8.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);
                stream.Close();

                return new OperationResult(Result.OK, recievedMessage.ToString());
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.ToString());
            }
        }
        /// <summary>
        /// Отправить сообщение на сервер
        /// </summary>
        /// <param name="message"> текст сообщения </param>
        public OperationResult SendMessageToServer(string message)
        {
            try
            {
                tcpClient = new TcpClient("127.0.0.1", 8080);
                NetworkStream stream = tcpClient.GetStream();

                byte[] msgHeader = new byte[20];//20 - первых байт на заголовок;
                Encoding.UTF8.GetBytes("msg@--@:").CopyTo(msgHeader, 0);
                byte[] msgContent = Encoding.UTF8.GetBytes(message);
                byte[] msgBuffer = new byte[msgContent.Length + msgHeader.Length];
                msgHeader.CopyTo(msgBuffer, 0);
                msgContent.CopyTo(msgBuffer, msgHeader.Length);

                //byte[] data = Encoding.UTF8.GetBytes("msg@:" + message);
                stream.Write(msgBuffer, 0, msgBuffer.Length);
                stream.Close();
                tcpClient.Close();
                return new OperationResult(Result.OK, "");
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
        }

        /// <summary>
        /// Отправить подтверждение на сервер
        /// </summary>
        /// <param name="message"> текст сообщения </param>
        public OperationResult SendAckToServer(string message)
        {
            try
            {
                tcpClient = new TcpClient("127.0.0.1", 8080);
                NetworkStream stream = tcpClient.GetStream();

                byte[] msgHeader = new byte[20];//20 - первых байт на заголовок;
                Encoding.UTF8.GetBytes("ack@--@:").CopyTo(msgHeader, 0);
                byte[] msgContent = Encoding.UTF8.GetBytes(message);
                byte[] msgBuffer = new byte[msgContent.Length + msgHeader.Length];
                msgHeader.CopyTo(msgBuffer, 0);
                msgContent.CopyTo(msgBuffer, msgHeader.Length);

                //byte[] data = Encoding.UTF8.GetBytes("msg@:" + message);
                stream.Write(msgBuffer, 0, msgBuffer.Length);
                stream.Close();
                tcpClient.Close();
                return new OperationResult(Result.OK, "");
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
        }

        /// <summary>
        /// Отправить файл на сервер
        /// </summary>
        /// <param name="FilePath"> путь к файлу </param>
        public OperationResult SendFileToServer(string FilePath)
        {
            try
            {
                tcpClient = new TcpClient("127.0.0.1", 8080);
                NetworkStream stream = tcpClient.GetStream();//Получаем текущий поток данных
                FileInfo fi = new FileInfo(FilePath);
                Stream fileStream = File.OpenRead(FilePath);//Читаем файл
                //byte[] fileBuffer = Encoding.UTF8.GetBytes("txt@:" 
                byte[] fileHeader = new byte[20];//20 - первых байт на заголовок;
                Encoding.UTF8.GetBytes("file@-" + fi.Extension + "-@:").CopyTo(fileHeader, 0);
                byte[] fileContent = new byte[fileStream.Length];
                byte[] fileBuffer = new byte[fileHeader.Length + fileContent.Length];
                fileHeader.CopyTo(fileBuffer, 0);
                fileContent.CopyTo(fileBuffer, fileHeader.Length);
                fileStream.Read(fileBuffer, fileHeader.Length, (int)fileStream.Length);//Записваем файл в виде байтов информации

                stream.Write(fileBuffer, 0, fileBuffer.GetLength(0));
                stream.Close();
                tcpClient.Close();
                return new OperationResult(Result.OK, "");
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
        }

        public void CloseClient()
        {
            tcpClient.Close();
        }
    }
}
