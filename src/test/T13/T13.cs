// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;
using System.Threading.Tasks;


namespace T13
{
    class T13
    {
        static void Main(string[] args)
        {
            B b = new B();
            Foo foo = new Foo();
            string s = "";
            A a = null;
            try
            {
                a = foo.FooEntry(b);
                foo.FooProcess(a);
            }
            catch (Exception e)
            {
                s = e.Message;
            }
            finally
            {
                B result = foo.FooDoAsync();
                Console.WriteLine("Done: {0}", s);
            }
            
        }
    }

    class Foo
    {
        public A FooEntry(B bpri)
        {
            B bsec = null;
            try
            {
                bsec = FooCallee();
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

        public B FooCallee()
        {
            B b = new B();
            if (b == null)
            {
                throw new NullReferenceException();
            }
            return b;
        }

        public void FooProcess(A a)
        {
            if (a == null) throw new NullReferenceException();
        }

        public B FooDoAsync()
        {
            Task<B> res = DoAsyncWork();
            res.Wait();
            return res.Result;
        }

        public async Task<B> DoAsyncWork()
        {
            B res = null;
            try
            {
                res = await MyAsyncTask();
            }
            catch (Exception e)
            {
                throw e;
            }
            return res;
        }

        public Task<B> MyAsyncTask()
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
