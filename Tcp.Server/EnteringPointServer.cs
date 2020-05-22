using System;
using System.Threading;
using SomeProject.Library.Server;

namespace SomeProject.TcpServer
{
    class EnteringPointServer
    {
        static void Main(string[] args)
        {
           try
            {
                Server server = new Server();
                Thread thr1 = new Thread(server.TurnOnListener);
                Thread thr2 = new Thread(server.TurnOnListener);
                thr1.Start();
                thr2.Start();
                //server.TurnOnListener().Wait();
                
                //server.turnOffListener();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
