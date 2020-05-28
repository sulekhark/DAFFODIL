// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿/******
Add the following code at the end of DoRTA() in ByteCodeAnalyzer.cs to test.
Then, you will have to set breakpoints at various points in the code below to test the behavior of SignaturesAreEqual.

 foreach (IMethodDefinition meth1 in rtaAnalyzer.methods)
            {
                foreach (IMethodDefinition meth2 in rtaAnalyzer.methods)
                {
                    bool signEq = MemberHelper.SignaturesAreEqual(meth1, meth2);
                }
            }

To test for generic method instances, add the following code:
 foreach (IMethodDefinition meth1 in rtaAnalyzer.methods)
            {
                foreach (IMethodDefinition meth2 in rtaAnalyzer.methods)
                {
                    if (meth1 is IGenericMethodInstance && meth2 is IGenericMethodInstance)
                    {
                        bool signEq = MemberHelper.GenericMethodSignaturesAreEqual(meth1, meth2);
                        IGenericMethodInstance gm1 = meth1 as IGenericMethodInstance;
                        IGenericMethodInstance gm2 = meth2 as IGenericMethodInstance;
                        IList<ITypeReference> genArgs1 = gm1.GenericArguments.ToList();
                        IList<ITypeReference> genArgs2 = gm2.GenericArguments.ToList();
                        if (genArgs1.Count() == genArgs2.Count())
                        {
                            for (int i = 0; i < genArgs1.Count(); i++)
                            {
                                if (genArgs1[i].InternedKey != genArgs2[i].InternedKey)
                                {
                                    signEq = false;
                                    break;
                                }
                            }
                        }
                        else signEq = false;
                    }
                }
            }

******/
namespace SignatureExpt.NSCommon
{
    class MyObj { }

    class MyWrapper
    {
        MyObj wrappedObj;

        public MyWrapper(MyObj paramObj)
        {
            wrappedObj = paramObj;
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            NSFirst.A firstA = new NSFirst.A();
            firstA.Foo(null);
            Baz<string>(null);
            Baz<MyObj>(null);
            Taz<string>(null);
            MyWrapper mainWrapperObj = firstA.Bar(5);

            NSSecond.A secondA = new NSSecond.A();
            secondA.Foo();
            mainWrapperObj = secondA.Bar(6);
        }

        private static void Baz<T>(string[] hello)
        {
            T sample = default(T);
            System.Console.WriteLine(sample.ToString());
            NSCommon.MyObj fooObj = new NSCommon.MyObj();
            NSCommon.MyWrapper fooWrapper = new NSCommon.MyWrapper(fooObj);
        }

        private static void Taz<T>(string[] hello)
        {
            NSCommon.MyObj fooObj = new NSCommon.MyObj();
            NSCommon.MyWrapper fooWrapper = new NSCommon.MyWrapper(fooObj);
        }
    }
}