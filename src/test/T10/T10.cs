// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;
using System.Collections.Generic;
namespace T10
{
    class Program
    {
        static void Main(string[] args)
        {
            MyObj<string> xxx = new MyObj<string>("hello");
            string yyy = xxx.GetVal();
        }
    }

    class MyObj<T>
    {
        T elem;

        public MyObj(T e)
        {
            elem = e;
        }

        public T GetVal()
        {
            T x = foo();
            return x;
        }

        T foo()
        {
            Exception ex = new Exception();
            Exception ex1 = ex.GetBaseException();
            return bar();
        }

        T bar()
        {
            return elem;
        }
    }
}
