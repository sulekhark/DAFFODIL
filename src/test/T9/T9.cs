// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿
namespace T9
{
    class T9
    {
        // delegate declaration
        public delegate bool printString(string s1, string s2, int i);
        static void Main(string[] args)
        {
            MyObjExt x1 = new MyObjExt();
            printString p1 = makeDelegate1(x1);
            sendString(p1);

            MyAbsObjExt x2 = new MyAbsObjExt();
            printString p2 = makeDelegate2(x2);
            sendString(p2);

            MyObjIExt x3 = new MyObjIExt();
            printString p3 = makeDelegate3(x3);
            sendString(p3);
        }

        public static void sendString(printString ps)
        {
            bool success = ps("Hello World", "foo", 2);
        }

        static printString makeDelegate1(MyObj x)
        {
            printString ps = new printString(x.writeStr);
            return ps;
        }

        static printString makeDelegate2(MyAbsObj x)
        {
            printString ps = new printString(x.writeStr);
            return ps;
        }

        static printString makeDelegate3(IMyObj x)
        {
            printString ps = new printString(x.writeStr);
            return ps;
        }
    }

    class MyObj
    {
        public virtual bool writeStr(string s1, string s2, int i)
        {
            System.Console.WriteLine("From MyObj: {0}, {1}", s1, s2);
            return true;
        }
    }

    class MyObjExt : MyObj
    {
        public override bool writeStr(string s1, string s2, int i)
        {
            System.Console.WriteLine("From MyObjExt: {0}, {1}", s1, s2);
            return true;
        }
    }

    abstract class MyAbsObj
    {
        public abstract bool writeStr(string s1, string s2, int i);
    }

    class MyAbsObjExt : MyAbsObj
    {
        public override bool writeStr(string s1, string s2, int i)
        {
            System.Console.WriteLine("From MyAbsObjExt: {0}, {1}", s1, s2);
            return true;
        }
    }

    interface IMyObj
    {
        bool writeStr(string s1, string s2, int i);
    }

    class MyObjIExt : IMyObj
    {
        public bool writeStr(string s1, string s2, int i)
        {
            System.Console.WriteLine("From MyObjIExt: {0}, {1}", s1, s2);
            return true;
        }
    }
}
