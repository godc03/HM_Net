using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using MsgDef;
using System.IO;

namespace ClientDemo
{
    class Client3
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
            object obj = Marshal.PtrToStructure(structPtr, type);
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }

        public void SendMsg(Socket socket,MsgDef.MSG_Type type, byte[] data)
        {
            byte[] sendBuf = new byte[1024];
            //add common header 
            byte[] tmp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data.Length));  //BE LE
            Array.Copy(tmp, sendBuf, tmp.Length);
            tmp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)type));
            Array.Copy(tmp, 0, sendBuf, 4, tmp.Length);

            //copy data
            Array.Copy(data, 0, sendBuf, 8, data.Length);
            socket.Send(sendBuf, sendBuf.Length, SocketFlags.None);
        }

        public void Start()
        {
            Console.WriteLine("Client3 start....");
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
            byte[] recvBuf = new byte[1024];
            int recvBufOffset = 0;
            int remainLen = 0;
            byte[] sendBuf = new byte[1024];
            //login
            Console.WriteLine("Input Accout:");
            String Accout = Console.ReadLine();
            Console.WriteLine("Input Password:");
            String Password = Console.ReadLine();
            sendBuf = Struct2Bytes(new MsgDef.MSG_Login(Accout, Password));
            //socket.Send(sendBuf, sendBuf.Length, SocketFlags.None);
            SendMsg(socket, MsgDef.MSG_Type.MSG_Login, sendBuf);
            //SendMsg(socket,MsgDef.MSG_Type.MSG_Login,Account,Password);

            int playerID = 0;
            int MsgHeaderLen = Marshal.SizeOf(new MsgDef.MSG_CommonHeard());
            //game loop
            while(true)
            {
                //解析消息结构体   //TODO 环形内存池 同步锁
                byte[] tmpBuf = new byte[1024];
                int recvLen = socket.Receive(tmpBuf);
                Array.Copy(tmpBuf, 0, recvBuf, recvBufOffset, recvLen);
                remainLen += recvLen;
                recvBufOffset += recvLen;
                //new Thread(() => {} ).Start();
                //作业：参照服务端 把消息解析用多线程实现  建议使用 TcpClient AsynConnect AsynSend 等封装
                while (remainLen > MsgHeaderLen)
                {
                    MSG_CommonHeard msg = new MSG_CommonHeard();
                    byte[] headerData = new byte[MsgHeaderLen];
                    Array.Copy(recvBuf, headerData, MsgHeaderLen);
                    msg.Len = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerData, 0));
                    msg.Type = (MsgDef.MSG_Type)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerData, 4));
                    //完整消息
                    if (remainLen - MsgHeaderLen >= msg.Len)
                    {
                        byte[] msgBuf = new byte[msg.Len];
                        Array.Copy(recvBuf, MsgHeaderLen, msgBuf, 0, msg.Len);
                        //remove current message from buffer
                        int curMsgLen = MsgHeaderLen + msg.Len;
                        Array.Copy(recvBuf, curMsgLen, recvBuf, 0, remainLen - curMsgLen);
                        recvBufOffset -= curMsgLen;
                        remainLen -= curMsgLen;

                        //解析消息
                        switch (msg.Type)
                        {
                            case MSG_Type.MSG_Login:
                                MSG_Login msg_login = new MSG_Login("", "");
                                msg_login = (MSG_Login)Bytes2Struct(msgBuf, msg_login.GetType());
                                string playerName = new string(msg_login.Account).TrimEnd('\0');
                                playerID = msg_login.PlayerID;
                                Console.WriteLine("Receive Message: {0} Login. ID:{1} ", playerName,msg_login.PlayerID);
                                break;
                            case MSG_Type.MSG_Move:
                                MSG_Move msg_move = new MSG_Move(0, 0, 0);
                                msg_move = (MSG_Move)Bytes2Struct(msgBuf, msg_move.GetType());
                                Console.WriteLine("Receive Message: {0} Move to {1}:{2} ", msg_move.PlayerID, msg_move.Pos_X, msg_move.Pos_Y);
                                break;
                            case MSG_Type.MSG_Quit:
                                MSG_Quit msg_quit = new MSG_Quit(0);
                                msg_quit = (MSG_Quit)Bytes2Struct(msgBuf, msg_quit.GetType());
                                Console.WriteLine("Receive Message: {0} Quit ", msg_quit.PlayerID);
                                break;
                            default:
                                break;
                        }
                    }
                }

                Console.WriteLine("Choose action: \n 1:Move \n 2 Quit \n 3 Refresh");
                String inputString = Console.ReadLine();
                if(inputString == "1")
                {
                    int x = 0, y = 0;
                    //TODO check Input
                    Console.WriteLine("Please input x:");
                    String inputX = Console.ReadLine();
                    x = Int32.Parse(inputX);

                    Console.WriteLine("Please input y:");
                    String inputY = Console.ReadLine();
                    y = Int32.Parse(inputY);

                    sendBuf = Struct2Bytes(new MsgDef.MSG_Move(playerID ,x, y));
                    ///socket.Send(sendBuf, sendBuf.Length, SocketFlags.None);
                    SendMsg(socket, MsgDef.MSG_Type.MSG_Move, sendBuf);
                }
                else if(inputString == "2")
                {
                    sendBuf = Struct2Bytes(new MsgDef.MSG_Quit(playerID));
                    //socket.Send(sendBuf, sendBuf.Length, SocketFlags.None);
                    SendMsg(socket, MsgDef.MSG_Type.MSG_Quit, sendBuf);
                    break;
                }
                else if(inputString == "3")
                {
                    //for recieve message 
                    continue;
                }
                else
                {
                    Console.WriteLine("Error input please choose again!");
                    continue;
                }
            }

            Console.WriteLine("Disconnect!");
            socket.Shutdown(SocketShutdown.Both);   //receive send both
            socket.Close();
            Console.ReadKey();
        }
    }
}