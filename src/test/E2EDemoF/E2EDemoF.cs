// Copyright (c) Sulekha Kulkarni.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.

﻿using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

namespace E2EDemoF
{
    class E2EDemoF
    {
        static FileStream fs;
        static StreamWriter sw;

        // delegate declaration
        public delegate bool printString(string del1Param, string del2Param, int i);

        // this method prints to the console
        public static bool WriteToScreen(string str1Param, string str2Param, int i)
        {
            Console.WriteLine("The String is: {0}", str1Param);
            if (str2Param == null) throw new ArgumentNullException("Second arg null.");
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

        public static void sendString(printString psDelVar)
        {
            bool success = psDelVar("Hello World", "foo", 2);
        }

        static void Main(string[] args)
        {
            B b = new B();
            Foo foo = new Foo();
            string s = "Everything fine.";
            A a;
            try
            {
                a = foo.FooOne(b);
                foo.FooTwo(a);
                printString ps1 = new printString(WriteToScreen);
                printString ps2 = new printString(WriteToFile);
                printString ps = ps1;
                ps += ps2;
                sendString(ps);
            }
            catch (Exception e)
            {
                s = e.Message;
            }
            finally
            {
                foo.DoBar();
                Console.WriteLine("Done: {0}", s);
            }
            
        }
    }

    class Foo
    {
        public A FooOne(B bpri)
        {
            B bsec;
            try
            {
                bsec = FooThree();
            }
            catch (NullReferenceException nre)
            {
                throw nre;
            }
            A a = new A();
            a.pri = bpri;
            a.sec = bsec;
            return a;
        }

        public B FooThree()
        {
            B b = new B();
            if (b == null)
            {
                throw new NullReferenceException();
            }
            return b;
        }

        public void FooTwo(A a)
        {
            if (a == null) throw new NullReferenceException();
        }

        public B DoBar()
        {
            string s = "hello";
            Task<B> res = DoAsyncWork(s);
            res.Wait();
            return res.Result;
        }

        public async Task<B> DoAsyncWork(string strParam)
        {
            B res2 = null;
            int res1;
            try
            {
                res1 = await MyTask1(strParam);
                if (res1 == 0)
                {
                    res2 = await MyTask2();
                }
                
            }
            catch (Exception e)
            {
                throw e;
            }
            return res2;
        }

        public Task<int> MyTask1(string sp)
        {
            IList<string> lclList = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                lclList.Add(sp);
            }
            Task<int> itsk;
            int val = 0;
            itsk = Task.Run(() =>
            {
                foreach (string member in lclList) val = Process(val, member);
                return val;
            });
            return itsk;
        }

        public int Process(int val, string s)
        {
            int curr = val;
            curr += s.Length;
            if (val > 10) throw new Exception("Value beyond upper limit.");
            return curr;
        }
        public Task<B> MyTask2()
        {
            Task<B> tmo;
            B xxx;
            tmo = Task.Run(() =>
            {
                xxx = new B();
                if (xxx == null) throw new DivideByZeroException();
                return xxx;
            });
            return tmo;
        }
    }

    class A
    {
        public B pri;
        public B sec;
    }

    class B { }
}
