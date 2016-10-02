using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;


namespace ServerDemo
{
    class Server1
    {
        public void Start()
        {
            Console.WriteLine("Server1 Start... \n");

            //socket bind listen 
            Socket ListernerSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, 20168);
            ListernerSocket.Bind(ipEnd);
            ListernerSocket.Listen(10);
            Console.WriteLine("Waiting for connent....");

            //while
            while(true)
            {
                Socket ClientSocket = ListernerSocket.Accept();
                IPEndPoint ipEndClient = (IPEndPoint)ClientSocket.RemoteEndPoint;
                Console.WriteLine("Connect with {0} : {1} ", ipEndClient.Address, ipEndClient.Port);

                byte[] sendBuf = new byte[1024];
                string msg = "Welcome!";
                sendBuf = Encoding.ASCII.GetBytes(msg);                           
                int sendLen = ClientSocket.Send(sendBuf, sendBuf.Length, SocketFlags.None);

                while(true)
                {
                    //receive
                    byte[] recvBuf = new byte[1024];
                    int recLen = ClientSocket.Receive(recvBuf);
                    string recvMsg = Encoding.ASCII.GetString(recvBuf, 0, recLen);
                    Console.WriteLine("Receive message :{0}", recvMsg);

                    //do something
                    string sendMsg = recvMsg.ToUpper();

                    //send
                    sendBuf = Encoding.ASCII.GetBytes(sendMsg);
                    ClientSocket.Send(sendBuf, sendBuf.Length, SocketFlags.None);

                    if (recvMsg.CompareTo("bye") == 0)
                        break;
                }

                Console.WriteLine("Disconnect of {0} ", ipEndClient.Address);
                ClientSocket.Close();
            }

            //nerver run here
            ListernerSocket.Close();
        }
    }
}