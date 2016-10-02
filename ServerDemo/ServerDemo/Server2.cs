using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using System.Runtime.InteropServices;
/*
namespace ServerDemo
{
    public enum MSG_Type
    {
        MSG_None = 0,
        MSG_Login = 1,
        MSG_Move = 2,
    }

    public struct MSG_CommonHeard
    {
        public int Len;
        public MSG_Type Type;
    }

    public struct MSG_Login
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] Account;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] Password;
        public MSG_Login(string accout, string password)
        {
            this.Account = accout.PadRight(32, '\0').ToCharArray();
            this.Password = password.PadRight(32, '\0').ToCharArray();
        }
    }

    public struct MSG_Move
    {
        public int PlayerID;
        public int Pos_X;
        public int Pos_Y;
    }


    class Server2
    {

        public byte[] Struct2Bytes(object obj)
        {
            int size = Marshal.SizeOf(obj);
            byte[] buf = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, structPtr, false);
            Marshal.Copy(structPtr, buf, 0, size);
            Marshal.FreeHGlobal(structPtr);
            return buf;
        }

        public object Bytes2Struct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length)
            { return null; }
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, structPtr, size);
            object obj = Marshal.PtrToStructure(structPtr,type);
            Marshal.FreeHGlobal(structPtr);

            return obj;
        }

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

                while(true)
                {
                    //receive
                    byte[] recvBuf = new byte[1024];
                    int recLen = ClientSocket.Receive(recvBuf); //TODO 处理粘包 //异常处理

                    MSG_Login msg = new MSG_Login("", "");
                    msg = (MSG_Login)Bytes2Struct(recvBuf, msg.GetType());

                    //do something
                    string accout = new string(msg.Account);
                    string password = new string(msg.Password);

                    string sendMsg = "fail";
                    if (accout.CompareTo("admin") == 0 && password.CompareTo("abc123") == 0)
                        sendMsg = "success";
                    else
                        Console.WriteLine("error accout or password  {0} {1} ", accout, password);
                    //send
                    sendBuf = Encoding.ASCII.GetBytes(sendMsg);
                    ClientSocket.Send(sendBuf, sendBuf.Length, SocketFlags.None);

                    if (sendMsg.CompareTo("success") == 0)
                        break;
                }

                Console.WriteLine("Disconnect of {0} ", ipEndClient.Address);
                Console.ReadKey();
                ClientSocket.Close();
            }

            //nerver run here
            ListernerSocket.Close();
        }
    }
}
*/