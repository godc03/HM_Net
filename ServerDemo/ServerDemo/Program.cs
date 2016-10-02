using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Server3 server = new Server3();
            server.Start();
             //socket
            /*
            Console.WriteLine("Server Start... \n");
            Socket ListernerSocket = new Socket(AddressFamily.InterNetwork, //ip4 ip6
                                            SocketType.Stream,              //stream raw
                                            ProtocolType.Tcp);              // tcp udp

            //bind
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, 20168);        //ip + port
            ListernerSocket.Bind(ipEnd);                                    //bind

            //listen & accept
            ListernerSocket.Listen(10);                                     //10 backlog
            Console.WriteLine("Waiting for connent....");
            Socket ClientSocket = ListernerSocket.Accept();

            IPEndPoint ipEndClient = (IPEndPoint)ClientSocket.RemoteEndPoint;
            Console.WriteLine("Connect with {0} : {1} ", ipEndClient.Address, ipEndClient.Port);

            //send
            byte[] sendBuf = new byte[1024];
            string msg = "Welcome!";
            sendBuf = Encoding.ASCII.GetBytes(msg);                            //big endian little endian
            int sendLen = ClientSocket.Send(sendBuf, sendBuf.Length, SocketFlags.None);

            //receive
            byte[] recvBuf = new byte[1024];
            int recLen = ClientSocket.Receive(recvBuf);
            string recvMsg = Encoding.ASCII.GetString(recvBuf, 0, recLen);
            Console.WriteLine("Receive message :{0}", recvMsg);

            Console.WriteLine("Disconnect of {0} ", ipEndClient.Address);

            ClientSocket.Close();
            ListernerSocket.Close();


            Console.ReadKey();
            */

        }
    }
}
