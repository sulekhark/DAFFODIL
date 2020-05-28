// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;

namespace T6
{
    class T6
    {
        static void Main(string[] args)
        {
            bar();
        }
        public static void bar()
        {
            Console.WriteLine(foo(x => x + 1, 3));
        }
        static int foo(Func<int, int> f, int a)
        {
            return f(a);
        }
    }
}
