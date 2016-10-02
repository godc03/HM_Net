using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MsgDef
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
        public MSG_CommonHeard(int len, MSG_Type type)
        {
            this.Len = len;
            this.Type = type;
        }
    }

    public struct MSG_Login
    {
        public int PlayerID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] Account;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] Password;

        public MSG_Login(string accout, string password)
        {
            this.PlayerID = 0;
            this.Account = accout.PadRight(32, '\0').ToCharArray();
            this.Password = password.PadRight(32, '\0').ToCharArray();
        }
    }

    public struct MSG_Move
    {
        public int PlayerID;
        public int Pos_X;
        public int Pos_Y;

        public MSG_Move(int id, int x, int y)
        {
            this.PlayerID = id;
            this.Pos_X = x;
            this.Pos_Y = y;
        }
    }
    public struct MSG_Quit
    {
        public int PlayerID;
        public MSG_Quit(int id)
        {
            this.PlayerID = id;
        }
    }
}