// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Cci;
using Microsoft.Cci.Immutable;

namespace Daffodil.DatalogAnalysisFW.AnalysisNetConsole
{
    public static class Stubs
    {
        static IDictionary<string, string> NameToNameMap;
        static IDictionary<string, ITypeDefinition> NameToTypeDefMap;
        private static IInternFactory internFactory;

        public static void Clear()
        {
            NameToNameMap.Clear();
            NameToTypeDefMap.Clear();
        }
        public static void SetupInternFactory(IInternFactory ifactory)
        {
            internFactory = ifactory;
        }

        public static void SetupStubs(IModule stubsModule)
        {
            NameToTypeDefMap = new Dictionary<string, ITypeDefinition>();
            NameToNameMap = new Dictionary<string, string>();
            NameToNameMap.Add("System.Threading.Tasks.VoidTaskResult", "Daffodil.Stubs.VoidTaskResult");
            NameToNameMap.Add("System.Runtime.CompilerServices.AsyncTaskMethodBuilder", "Daffodil.Stubs.AsyncTaskMethodBuilder");
            NameToNameMap.Add("System.Runtime.CompilerServices.AsyncTaskMethodBuilder<TResult>", "Daffodil.Stubs.AsyncTaskMethodBuilder<TResult>");
            NameToNameMap.Add("System.Threading.Tasks.Task", "Daffodil.Stubs.Task");
            NameToNameMap.Add("System.Threading.Tasks.Task<TResult>", "Daffodil.Stubs.Task<TResult>");
            NameToNameMap.Add("System.Runtime.CompilerServices.TaskAwaiter", "Daffodil.Stubs.TaskAwaiter");
            NameToNameMap.Add("System.Runtime.CompilerServices.TaskAwaiter<TResult>", "Daffodil.Stubs.TaskAwaiter<TResult>");
            NameToNameMap.Add("System.Runtime.CompilerServices.IAsyncStateMachine", "Daffodil.Stubs.IAsyncStateMachine");

            foreach (ITypeDefinition ty in stubsModule.GetAllTypes().OfType<INamedTypeDefinition>().ToList())
            {
                NameToTypeDefMap.Add(ty.FullName(), ty);
            }
        }

        static ITypeDefinition FindIfPresent (ITypeDefinition ty)
        {
            string clName = ty.FullName();
            if (NameToNameMap.ContainsKey(clName))
            {
                string stubClName = NameToNameMap[clName];
                if (NameToTypeDefMap.ContainsKey(stubClName))
                {
                    ITypeDefinition stubCl = Stubs.NameToTypeDefMap[stubClName];
                    if (stubCl != null) return stubCl;
                    System.Console.WriteLine("WARNING: stubCl should not have been null");
                }
                else
                {
                    System.Console.WriteLine("WARNING: Invalid state in StubMap");
                }
            }
            return null;
        }

        static ITypeDefinition CreateGenericTypeInstance(IGenericTypeInstance gty, ITypeDefinition stubTemplate)
        {
            INamedTypeDefinition nStubTemplate = stubTemplate as INamedTypeDefinition;
            IEnumerable<ITypeReference> genArgs = gty.GenericArguments;
            GenericTypeInstanceReference stubbedGtyRef =
                  new GenericTypeInstanceReference(stubTemplate as INamedTypeReference, genArgs, internFactory);
            ITypeDefinition stubbedGty = stubbedGtyRef.ResolvedType;
            NameToNameMap.Add(gty.FullName(), stubbedGty.FullName());
            NameToTypeDefMap.Add(stubbedGty.FullName(), stubbedGty);
            return stubbedGty;
        }

        public static ITypeDefinition GetStubType(ITypeDefinition ty)
        {
            ITypeDefinition stubTy = FindIfPresent(ty);
            if (stubTy != null) return stubTy;
           
            if (ty is IGenericTypeInstance)
            {
                IGenericTypeInstance gty = ty as IGenericTypeInstance;
                INamedTypeDefinition templateOfGty = gty.GenericType.ResolvedType;
                ITypeDefinition equivStubTemplate = FindIfPresent(templateOfGty);
                if (equivStubTemplate == null) return null;
                stubTy = CreateGenericTypeInstance(gty, equivStubTemplate);
                return stubTy;
            }
            return null;
        }
    }
}
