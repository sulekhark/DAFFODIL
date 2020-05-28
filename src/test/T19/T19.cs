// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;
using System.IO;

namespace T19
{

    class T19
    {
        static FileStream fs;
        static StreamWriter sw;
        Foo fooMember;

        // delegate declaration
        public delegate bool printString(string del1Param, string del2Param, int i);

        // this method prints to the console
        public bool WriteToScreen(string str1Param, string str2Param, int i)
        {
            A strAccess = fooMember.info;
            Console.WriteLine("The String is: {0}", str1Param);
            return true;
        }

        //this method prints to a file
        public static bool WriteToFile(string stat1Param, string stat2Param, int i)
        {
            fs = new FileStream(".\\message.txt", FileMode.Append, FileAccess.Write);
            sw = new StreamWriter(fs);
            sw.WriteLine(stat1Param);
            return true;
        }

        // this method takes the delegate as parameter and uses it to
        // call the methods as required
        public static void sendString(printString psDelVar)
        {
            bool success = psDelVar("Hello World", "foo", 2);
        }

        static void Main(string[] args)
        {
            A strMainVal = new A();
            Foo fooInst = new Foo(strMainVal);
            T19 mainTObj = new T19();
            mainTObj.fooMember = fooInst;
            printString ps1 = new printString(mainTObj.WriteToScreen);
            printString ps2 = new printString(WriteToFile);
           
            printString ps = ps1;
            ps += ps2;
            sendString(ps);
        }
    }

    class Foo
    {
        public A info;

        public Foo(A fooParam)
        {
            info = fooParam;
        }
    }

    class A { }
}