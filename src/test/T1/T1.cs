// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using T2_NS;
using System.Collections.Generic;

namespace T1
{
    public class Foo
    {
        WOut finalRes;
        public WOut Work(bool extend)
        {
            WOut1 result1;
            WOut2 result2; 
            Bar bar = new Bar();
            result1 = bar.Dojob1();
            finalRes = result1;
            if (extend)
            {
                result2 = bar.Dojob2(result1);
                finalRes = result2;
            }
            T2 temp = new T2();
            temp.foo_diff();
            System.Console.WriteLine("hello");
            ISet<int> mySet = new HashSet<int>();
            return finalRes;
        }

        public WOut GetRes() { return finalRes; }
    }

    public class Bar
    {
        public WOut1 Dojob1()
        {
            WOut1 res = new WOut1();
            return res;
        }

        public WOut2 Dojob2( WOut1 wo1)
        {
            WOut2 res = new WOut2();
            res.obj = wo1.obj;
            return res;
        }
    }

    public class WOut
    {
        public object obj;
    }

    public class WOut1 : WOut
    {
    }

    public class WOut2 : WOut
    {
    }
    
    public class MainCl
    {
        public static void Main()
        {
            Foo foo_obj = new Foo();
            foo_obj.Work(true);
        }
    }
}
    
