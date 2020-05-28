// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace T7NS
{

    class PString
    {
        static FileStream fs;
        static StreamWriter sw;
        string instStr;

        // delegate declaration
        public delegate bool printString(string s1, string s2, int i);

        // this method prints to the console
        public bool WriteToScreen(string str1Param, string str2Param, int i)
        {
            Console.WriteLine("The String is: {0} {1}", str1Param, instStr);
            return true;
        }

        //this method prints to a file
        public static bool WriteToFile(string s1, string s2, int i)
        {
            fs = new FileStream("c:\\message.txt",
            FileMode.Append, FileAccess.Write);
            sw = new StreamWriter(fs);
            sw.WriteLine(s1);
            sw.Flush();
            sw.Close();
            fs.Close();
            return true;
        }

        // this method takes the delegate as parameter and uses it to
        // call the methods as required
        public static void sendString(printString ps)
        {
            bool success = ps("Hello World", "foo", 2);
        }

        static void Main(string[] args)
        {
            PString p = new PString();
            printString ps1 = new printString(p.WriteToScreen);
            printString ps2 = new printString(WriteToFile);
            sendString(ps1);
            sendString(ps2);

            printString ps = ps1;
            ps += ps2;
            sendString(ps);

            Foo xxx;
            p.getFoo(out xxx);

            Bar bbb, ccc;
            bbb = new Bar(null, null);
            bbb.obj1 = xxx;
            bbb.obj2 = xxx;
            Bar fff = p.copy(out ccc, bbb);
            string s = ccc.PrintInfo();
            p.setDummy(out Bar.dum1);
            p.setDummy(out Foo.dum2);
            p.setDummy(out xxx.dum3);

        }

        void getFoo(out Foo xfoo)
        {
            xfoo = new Foo("msg1");
        }

        Bar copy(out Bar dest, Bar src)
        {
            dest = src;
            Bar ret = dest;
            return ret;
        }

        void setDummy(out Dummy d)
        {
            d = new Dummy();
        }
    }

    class Dummy { }
    class Foo
    {
        public string info;
        public static Dummy dum2;
        public Dummy dum3;

        public Foo(string s)
        {
            info = s;
        }
    }
    struct Bar
    {
        public Foo obj1;
        public Foo obj2;
        public static Dummy dum1;

        public Bar(Foo x1, Foo x2)
        {
            obj1 = x1;
            obj2 = x2;
        }
        public string PrintInfo()
        {
            System.Console.WriteLine(obj1.info);
            System.Console.WriteLine(obj2.info);
            return obj2.info;
        }
    }
}