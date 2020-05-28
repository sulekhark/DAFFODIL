// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿
namespace T12
{
    class T12
    {
        static void Main(string[] args)
        {
            Bar svar = new Bar(null, null);
            Foo fooFirst = new Foo();
            Foo zzz;
            FreshOutFoo(out zzz);
            FreshRefFoo(ref fooFirst);
            IBaz bazArg = new Baz();
            Taz tazArg = new Taz();
            fooFirst.FooFuncGenRef1<IBaz, Taz>(ref bazArg, ref tazArg);
            fooFirst.FooFuncTrial(ref bazArg, ref tazArg);
            Baz bazArgOrd = bazArg as Baz;
            fooFirst.FooFuncOrdinary(bazArgOrd, tazArg);
            fooFirst.FooFuncGenRef2<Baz, Taz>(ref bazArgOrd, ref tazArg);
            svar.objSec = fooFirst;
            Foo yyy = svar.objSec;
            Bar rvar = svar;
            rvar = Modify(rvar);
            LocMod(rvar);
            Bar freshBar = new Bar(null,null);
            FreshBar(out freshBar);
            FreshOutFoo(out freshBar.objFir);
            FreshOutFoo(out freshBar.objSec);
        }

        
        static void FreshBar(out Bar freshparam)
        {
            freshparam = new Bar(new Foo(), new Foo());
        }

        static void FreshRefFoo(ref Foo ff)
        {
            ff = new Foo();
        }

        static void FreshOutFoo(out Foo ff)
        {
            ff = new Foo();
        }

        static Bar Modify(Bar mod)
        {
            Foo orig = mod.GetF();
            if (orig == null)
            {
                Foo repl = new Foo();
                mod.SetF(repl);
            }
            return mod;
        }

        static void LocMod(Bar mod)
        {
            Foo orig = mod.GetF();
            if (orig == null)
            {
                Foo repl = new Foo();
                mod.SetF(repl);
            }
        }
    }
    struct Bar
    {
        public Foo objFir;
        public Foo objSec;

        public Bar(Foo x1, Foo x2)
        {
            objFir = x1;
            objSec = x2;
        }

        public Foo GetF()
        {
            return objFir;
        }

        public void SetF(Foo o)
        {
            objFir = o;
        }
    }

    class Foo
    {
        public static Bar[] barArray;
        public void FooFuncGenRef1<T1,T2>(ref T1 bazObj, ref T2 tazObj) where T1 : IBaz where T2 : Taz
        {
            tazObj.tazField = this;
            bazObj.BazFunc();
        }

        public void FooFuncGenRef2<T1, T2>(ref T1 bazObj, ref T2 tazObj) where T1 : Baz where T2 : Taz
        {
            bazObj.bazField = this;
            bazObj.BazFunc();
        }

        public void FooFuncTrial(ref IBaz bazObj, ref Taz tazObj)
        {
            tazObj.tazField = this;
            bazObj.BazFunc();
            bazObj.BazBaseFunc();
        }

        public void FooFuncOrdinary(Baz ordBaz, Taz ordTaz)
        {
            barArray = new Bar[10];
            ordTaz.tazField = this;
            ordTaz.TazBaseFunc();
            ordBaz.BazFunc();
        }
    }

    interface IBaz
    {
        void BazFunc();
        void BazBaseFunc();
    }

    class Baz : BazBase, IBaz
    {
        public Foo bazField;
        //public Taz tazMember;

        public Baz() { }
        public void BazFunc() { }
    }

    class BazBase
    {
        public void BazBaseFunc() { }
    }

    class Taz : TazBase
    {
        public Foo tazField;

        public Taz() { }
    }

    class TazBase
    {
        public void TazBaseFunc() { }
    }
}
