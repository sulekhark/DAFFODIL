// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T2_NS
{
    public class T2
    {
        public void foo_diff()
        {
            int x, y, w, z;

            x = 5;
            x = x - 3;
            if (x < 3)
            {
                y = x * 2;
                w = y;
            }
            else
            {
                y = x - 3;
            }
            w = x - y;
            z = x + y;
        }
    }

    public class NotReq
    {
        public void Foo()
        {
            try { Bar();}
            catch (Exception e)
            {
                Exception e1 = Check(e);
                if (e1 != null) throw e1;
            }
        }

        public void Bar()
        {
            try
            {
                FieldAccessException fae;
                fae = new FieldAccessException();
                throw fae;
            }
            catch (ArgumentNullException ane) { Console.WriteLine(ane.Message); }
        }

        public Exception Check(Exception e)
        {
            return e;
        }
    }
}
