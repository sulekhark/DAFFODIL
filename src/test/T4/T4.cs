// Author: Sulekha Kulkarni
// Date: Nov 2019

using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace FactTest
{
    static class AnonymousFunction
    {
        static WrapperObj fooAnon(Func<MyObj,WrapperObj> anonFuncParam, MyObj aaa)
        {
            WrapperObj fooWObj = anonFuncParam(aaa);
            return fooWObj;
        }
        public static void barAnon()
        {
            MyObj anonObj = new MyObj();
            WrapperObj barWObj = fooAnon(paramObj => new WrapperObj(paramObj), anonObj);
            MyObj finalObj = barWObj.mo;
        }
    }
    static class Iterator
    {
        static int counter = 0;
        static IEnumerable<MyObj> fooIt()
        {
            yield return bazIt();
           
        }

        public static void barIt()
        {
            foreach (MyObj loopObj in fooIt())
            {
                WrapperObj wx = new WrapperObj(loopObj);
                wx.Process();
            }
        }

        public static MyObj bazIt()
        {
            MyObj bazObj = new MyObj();
            counter++;
            return bazObj;
        }
    }
    class T4
    {
        public Task<MyObj> ShowAsync(bool flag)
        {
            MyObj xxx;
            Task<MyObj> tmo;

            if (flag)
            {
                tmo = Task.Run(() =>
                {
                    xxx = new MyObj();
                    Console.WriteLine("Hello");
                    xxx.i = 5;
                    return xxx;
                });
            }
            else
            {
                tmo = Task.Run(() =>
                {
                    xxx = new MyObj();
                    Console.WriteLine("Hello");
                    xxx.i = 7;
                    return xxx;
                });
            }
            return tmo;
        }
        public async Task<WrapperObj> CallAsync()
        {
            MyObj firstObj = new MyObj();
            WrapperObj wObj = new WrapperObj(firstObj);
            MyObj secondObj = null;
            try
            {
                secondObj = await ShowAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            wObj.mo = secondObj;
            return wObj;
        }
    }

    class MyObj
    {
        public int i;
       
    }

    class WrapperObj
    {
        public MyObj mo;

        public WrapperObj(MyObj arg)
        {
            mo = arg;
        }
        public void Process()
        {
            Console.WriteLine("Processing...");
        }
    }

    class Program
    {
        public static void Main(String[] args)
        {
            T4 demo = new T4();
            WrapperObj zzz = demo.CallAsync().Result;
            zzz.Process();
            Iterator.barIt();
            AnonymousFunction.barAnon();

            IDictionary<string, MyObj> dict;
            dict = new Dictionary<string, MyObj>();
            dict.Add("aaa", new MyObj());
            MyObj yyy = dict["aaa"];
        }
    }
}