using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;


namespace ClientDemo
{
    class Client1
    {
        public void Start()
        {
            Console.WriteLine("Client1 start....");
            //connect to server
            IPAddress serverIP = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEnd = new IPEndPoint(serverIP, 20168);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(ipEnd);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Failed to Connect :{0}", e.ToString());
            }

            while(true)
            {
                byte[] buf = new byte[1024];
                int recvLen = socket.Receive(buf);

                string recvMsg = Encoding.ASCII.GetString(buf, 0, recvLen);
                Console.WriteLine("Receive message: {0}", recvMsg);

                String inputMsg = Console.ReadLine();
                buf = Encoding.ASCII.GetBytes(inputMsg);
                socket.Send(buf, buf.Length, SocketFlags.None);

                if (inputMsg.CompareTo("bye") == 0)
                    break;
            }
            Console.WriteLine("Disconnect!");
            socket.Shutdown(SocketShutdown.Both);   //receive send both
            socket.Close();
        }
    }
}