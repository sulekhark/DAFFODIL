// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿
namespace T18
{
    public class T18
    {
        static void Main(string[] args)
        {
            A aaa = new A();
            MyObj ooo = new MyObj(aaa);
            A bbb = ooo.MyValue;
        }
    }

    public class MyObj
    {
        private A val;
        private int count;

        public A MyValue
        {
            get
            {
                count++;
                return val;
            }
        }
           
        public MyObj (A ip)
        {
            val = ip;
            count = 0;
        }

    }

    public class A { }
} 
