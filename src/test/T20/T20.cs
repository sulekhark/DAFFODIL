// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿namespace T20
{
    class T20
    {
        static void Main(string[] args)
        {
            FooBase fooB = new FooBase();
            Process(fooB);
            FooDerived fooD = new FooDerived();
            Process(fooD);
        }

        static void Process(IFooBase fooParam)
        {
            FooDerived fooParamD = fooParam as FooDerived;
            if (fooParamD != null) fooParamD.FooDFunc();
            fooParam.FooBFunc();
        }
    }

    interface IFooBase
    {
        void FooBFunc();
    }

    interface IFooDerived : IFooBase
    {
        void FooDFunc();
    }

    class FooBase : IFooBase
    {
        public void FooBFunc() { }
    }

    class FooDerived : IFooDerived
    {
        public void FooBFunc() { }
        public void FooDFunc() { }
    }
}
