// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;

namespace T15
{
    class T15
    {
        static void Main(string[] args)
        {
            B b = new B();
            Foo foo = new Foo();
            string s = "";
            A a;
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
                B[] result = foo.FooDoMore();
                for (int i = 0; i < 10; i++) result[i].BProcess();
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
            a.AProcess();
        }

        public B[] FooDoMore()
        {
            B[] arrayB = new B[10];
            for (int i = 0; i < 10; i++) arrayB[i] = new B();
            return arrayB;
        }
    }

    struct A
    {
        public B pri;
        public B sec;

        public void AProcess()
        {
            if (pri == null)
            {
                throw new NullReferenceException();
            }
        }
    }

    class B
    {
        public void BProcess()
        {
            throw new NullReferenceException();
        }
    }
}
