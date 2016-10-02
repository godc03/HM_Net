using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using System.Runtime.InteropServices;
/*
namespace ClientDemo
{
    public enum MSG_Type
    {
        MSG_None = 0,
        MSG_Login = 1,
        MSG_Move = 2,
        MSG_Quit = 3,
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
            this.Account = accout.PadRight(32,'\0').ToCharArray();
            this.Password = password.PadRight(32,'\0').ToCharArray();
        }
    }

    public struct MSG_Move
    {
        public int PlayerID;
        public int Pos_X;
        public int Pos_Y;
    }

    class Client2
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

                byte[] sendBuf = new byte[1024];

                Console.WriteLine("Input Accout:");
                String Accout = Console.ReadLine();
                Console.WriteLine("Input Password:");
                String Password = Console.ReadLine();
                sendBuf = Struct2Bytes(new MSG_Login(Accout, Password));
                socket.Send(sendBuf, sendBuf.Length, SocketFlags.None);

                byte[] recvBuf = new byte[1024];
                int recvLen = socket.Receive(recvBuf);
                string recvMsg = Encoding.ASCII.GetString(recvBuf, 0, recvLen);
                Console.WriteLine("Receive message: {0}", recvMsg);

                if (recvMsg.CompareTo("success")==0)
                    break;
            }

            Console.WriteLine("Login Success!");
            Console.ReadKey();
            Console.WriteLine("Disconnect!");
            socket.Shutdown(SocketShutdown.Both);   //receive send both
            socket.Close();
        }
    }
}
*/