using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections; //Hashtable

using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using MsgDef;

namespace ServerDemo
{
    class Server3
    {
        static Hashtable g_playerTable = new Hashtable();
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

        public void SendMsg(NetworkStream ns, MsgDef.MSG_Type type, byte[] data)
        {
            byte[] sendBuf = new byte[1024];
            //add common header 
            byte[] tmp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data.Length));  //BE LE
            Array.Copy(tmp, sendBuf, tmp.Length);
            tmp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)type));
            Array.Copy(tmp, 0, sendBuf, 4, tmp.Length);

            //copy data
            Array.Copy(data, 0, sendBuf, 8, data.Length);
            ns.Write(sendBuf, 0, 8 + data.Length);
        }

        public void BroadCast(MSG_Type type, byte[] data)
        {
            foreach (DictionaryEntry de in g_playerTable)
            {
                NetworkStream ns = (NetworkStream)de.Key;
                SendMsg(ns, type, data);
            }
        }

        public bool ProcessMsg(MSG_Type type, byte[] data, NetworkStream netStream)
        {
            bool bQuit = false;
            //process
            switch(type)
            {
                case MSG_Type.MSG_Login:
                    MSG_Login msg_login = new MSG_Login("", "");
                    msg_login = (MSG_Login)Bytes2Struct(data, msg_login.GetType());
                    msg_login.PlayerID = netStream.GetHashCode();
                    data = Struct2Bytes(msg_login);
                    g_playerTable.Add(netStream, msg_login);
                    string playerName = new string(msg_login.Account).TrimEnd('\0');
                    Console.WriteLine("{0} Login,PlayerID: {1} ", playerName, msg_login.PlayerID);
                    break;
                case MSG_Type.MSG_Move:
                    MSG_Move msg_move = new MSG_Move(0,0,0);
                    msg_move = (MSG_Move)Bytes2Struct(data, msg_move.GetType());
                    Console.WriteLine("{0} Move to {1}:{2} ", ((MSG_Login)g_playerTable[netStream]).PlayerID, msg_move.Pos_X, msg_move.Pos_Y);
                    break;
                case MSG_Type.MSG_Quit:
                    MSG_Quit msg_quit = new MSG_Quit(0);
                    msg_quit = (MSG_Quit)Bytes2Struct(data, msg_quit.GetType());
                    g_playerTable.Remove(netStream);
                    Console.WriteLine("{0} Quit ", msg_quit.PlayerID);
                    bQuit = true;
                    break;
                default:
                    bQuit = true;
                    break;
            }

            //broadcast
            BroadCast(type, data);

            return bQuit;
        }

        public void Start()
        {
            Console.WriteLine("Server3 Start... \n");
            //TcpListener
            TcpListener ListernerSocket = new TcpListener(IPAddress.Any, 20168);
            ListernerSocket.Start();

            //while
            while(true)
            {
                //Socket ClientSocket = ListernerSocket.Accept();   BeginAccept方法和EndAccept方法 BeginRecive方法和EndRecive方法
                TcpClient ClientSocket = ListernerSocket.AcceptTcpClient();
                NetworkStream netStream = ClientSocket.GetStream();
                Console.WriteLine("new client Connected ");

                new Thread(() => {
                    byte[] sendBuf = new byte[1024];
                    int MsgHeaderLen = Marshal.SizeOf(new MsgDef.MSG_CommonHeard());
                    while(true)
                    {
                        //receive
                        byte[] totalBuf = new byte[0];
                        byte[] recvBuf = new byte[1024];
                        try
                        {
                            int recLen = netStream.Read(recvBuf, 0, recvBuf.Length);
                            if(recLen > 0)
                            {
                                Array.Resize(ref totalBuf, totalBuf.Length + recLen);
                                Array.Copy(recvBuf, 0, totalBuf, totalBuf.Length - recLen, recLen);
                                if(totalBuf.Length > MsgHeaderLen)
                                {
                                    byte[] headerData = new byte[MsgHeaderLen];
                                    Array.Copy(totalBuf, headerData, MsgHeaderLen);
                                    MSG_CommonHeard msg = new MSG_CommonHeard();
                                    msg.Len = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerData, 0));
                                    msg.Type = (MsgDef.MSG_Type)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerData, 4));

                                    if (totalBuf.Length - MsgHeaderLen >= msg.Len)    //收到完整消息 则解析
                                    {
                                        byte[] msgData = new byte[msg.Len];
                                        Array.Copy(totalBuf.ToArray(),MsgHeaderLen,msgData,0,msg.Len);
                                        bool bQuit = ProcessMsg(msg.Type, msgData, netStream);
                                        //clear buffer
                                        Array.Copy(totalBuf, 0, totalBuf, msg.Len, totalBuf.Length - msg.Len);
                                        if (bQuit)
                                            break;
                                    }
                                }
                            }
                        }
                        catch(Exception E)
                        {
                            g_playerTable.Remove(netStream);
                            Console.WriteLine("Error: {1}",ClientSocket,E.Message );
                            break;
                        }
                        
                    }
                } ).Start();
            }

            //ListernerSocket.Stop();
        }
    }
}