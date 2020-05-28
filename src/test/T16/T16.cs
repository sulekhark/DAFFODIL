// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T16
{
    class T16
    {
        static void Main(string[] args)
        {
            Foo foo;
            GetFoo(out foo);
            Str bar;
            bar.fld = foo;
            Str baz = bar;
            Foo taz = bar.fld;
            // bar.dummy();
        }

        public static void GetFoo(out Foo fooP)
        {
            fooP = new Foo();
        }
    }

    class Foo { }
   
    struct Str
    {
        public Foo fld;
        // public void dummy() { }
    }
   
}
