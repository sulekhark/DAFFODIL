// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System.Collections.Generic;
using Microsoft.Cci;

namespace Daffodil.DatalogAnalysisFW.AnalysisNetConsole
{
    class Stubber
    {
        // NOTE: The way the "suppress" feature is designed, if System.Int32 is suppressed,
        //       so will System.Int32[] and System.Int32&.
        
        // For the classes whose names have prefixes in the list below:
        //    1. We will only not analyze methods in these classes
        //    2. We track objects of this type.
        //    3. We track reads/writes to static fields of these classes and instance fields of instances of these classes.
        //    4. We track flow of points-to information through local variables of these types.
        private static IList<string> prefixesToSuppressMeth;
        // For the classes whose names have prefixes in the list below:
        //    1. They are completely suppressed - we don't analyze methods, don't track fields and objects.
        //    2. We don't track flow of points-to information through variables of these types.
        //    3. They are not recorded in the list of "classes" to be analyzed.
        private static IList<string> prefixesToSuppressFull;

        private static RTAAnalyzer rtaAnalyzer;

        static Stubber()
        {
            prefixesToSuppressMeth = new List<string>();
            prefixesToSuppressMeth.Add("System.Console"); // Has bytecode arglist
            prefixesToSuppressMeth.Add("System.String");  // causes crash in TypeInference
            prefixesToSuppressMeth.Add("System.Environment");
            prefixesToSuppressMeth.Add("System.ComponentModel");
            prefixesToSuppressMeth.Add("System.Configuration");
            prefixesToSuppressMeth.Add("System.Data");
            prefixesToSuppressMeth.Add("System.Diagnostics");
            prefixesToSuppressMeth.Add("System.Globalization");
            prefixesToSuppressMeth.Add("System.IO");
            prefixesToSuppressMeth.Add("System.Linq.Expressions");
            prefixesToSuppressMeth.Add("System.Linq.Parallel");
            prefixesToSuppressMeth.Add("System.Net");
            prefixesToSuppressMeth.Add("System.Reflection");
            prefixesToSuppressMeth.Add("System.Resources");
            prefixesToSuppressMeth.Add("System.Runtime");
            prefixesToSuppressMeth.Add("System.Security");
            prefixesToSuppressMeth.Add("System.Text");
            prefixesToSuppressMeth.Add("System.Threading");
            prefixesToSuppressMeth.Add("System.Xml");
            prefixesToSuppressMeth.Add("System.Convert");
            prefixesToSuppressMeth.Add("Microsoft.Cci");

            prefixesToSuppressFull = new List<string>();
            prefixesToSuppressFull.Add("System.Double");
            prefixesToSuppressFull.Add("System.IntPtr");
            prefixesToSuppressFull.Add("System.Int64");
            prefixesToSuppressFull.Add("System.Int32");
            prefixesToSuppressFull.Add("System.Int16");
            prefixesToSuppressFull.Add("System.Byte");
            prefixesToSuppressFull.Add("System.SByte");
            prefixesToSuppressFull.Add("System.Single");
            prefixesToSuppressFull.Add("System.TokenType");
            prefixesToSuppressFull.Add("System.Char");
            prefixesToSuppressFull.Add("System.Boolean");
            prefixesToSuppressFull.Add("System.UIntPtr");
            prefixesToSuppressFull.Add("System.UInt64");
            prefixesToSuppressFull.Add("System.UInt32");
            prefixesToSuppressFull.Add("System.UInt16");
            prefixesToSuppressFull.Add("System.UChar");
            prefixesToSuppressFull.Add("System.Decimal");
        }

        public static void SetupRTAAnalyzer(RTAAnalyzer rta)
        {
            rtaAnalyzer = rta;
        }

        public static bool SuppressF(ITypeDefinition t)
        {
            bool matches = false;
            string tName = t.FullName();
            foreach (string s in prefixesToSuppressFull)
            {
                if (tName.StartsWith(s))
                {
                    matches = true;
                    break;
                }
            }
            return matches;
        }

        public static bool SuppressM(IMethodDefinition m)
        {
            return (GetMethodToAnalyze(m) == null);
        }

        public static bool SuppressM(ITypeDefinition ty)
        {
            bool retval = false;
            bool matches = MatchesSuppressM(ty);
            if (matches)
            {
                ITypeDefinition stubType = Stubs.GetStubType(ty);
                retval = stubType == null ? true : false;
            }
            return retval;
        }

        public static bool IsStubbed(ITypeDefinition ty)
        {
            bool retval = false;
            bool matches = MatchesSuppressM(ty);
            if (matches)
            {
                ITypeDefinition stubType = Stubs.GetStubType(ty);
                retval = stubType == null ? false : true;
            }
            return retval;
        }

        public static bool MatchesSuppressM(IMethodDefinition m)
        {
            string mSign = m.FullName();
            bool matches = false;
            foreach (string s in prefixesToSuppressMeth)
            {
                if (mSign.StartsWith(s))
                {
                    matches = true;
                    break;
                }
            }
            return matches;
        }

        public static bool MatchesSuppressM(ITypeDefinition t)
        {
            bool matches = false;
            string tName = t.FullName();
            foreach (string s in prefixesToSuppressMeth)
            {
                if (tName.StartsWith(s))
                {
                    matches = true;
                    break;
                }
            }
            return matches;
        }

        /****
           Four cases:
           1. Ordinary method (not stubbed and not generic)
           2. Stubbed but not generic
           3. Not stubbed but generic
           4. Stubbed and generic
           Note that there are two sub-cases for "is stubbed": "stub present" or "stub absent".
           So that makes it a total of 6 cases.
        ****/
        public static IMethodDefinition CheckAndAdd(IMethodDefinition m)
        {
            IMethodDefinition lookFor = m;
            if (m is IGenericMethodInstance)
            {
                lookFor = GenericMethods.GetTemplate(m);
            }
            
            ITypeDefinition containingType = m.ContainingTypeDefinition;
            if (containingType.InternedKey == 0) return m; // Ignore methods from Cci's Dummy typeref.
            bool matches = MatchesSuppressM(m);
            if (matches)
            {
                containingType = Stubs.GetStubType(containingType);
                if (containingType == null) return null; // This entire containingType is to be ignored.
                lookFor = Utils.GetMethodSignMatch(containingType, lookFor);
                if (lookFor == null) return null; // containingType itself is stubbed, but the stub does not define a method equivalent to m.
            }

            IMethodDefinition methToAdd;
            if (m is IGenericMethodInstance)
            {
                // Here, lookFor is a template method
                methToAdd = GenericMethods.RecordInfo(lookFor, m, matches);
            }
            else
            {
                methToAdd = lookFor;
            }
            if (!rtaAnalyzer.visitedMethods.Contains(methToAdd) && !rtaAnalyzer.methods.Contains(methToAdd))
            {
                rtaAnalyzer.rtaLogSW.WriteLine("SRK_DBG: Adding method: {0}", methToAdd.FullName());
                rtaAnalyzer.methods.Add(methToAdd);
            }
            return methToAdd;
        }

        public static ITypeDefinition CheckAndAdd(ITypeDefinition t)
        {
            if (SuppressF(t)) return null;
            if (t is IGenericTypeInstance)
            {
                IGenericTypeInstance gty = t as IGenericTypeInstance;
                IEnumerable<ITypeReference> genArgs = gty.GenericArguments;
                foreach (ITypeReference garg in genArgs) CheckAndAdd(garg.ResolvedType);
            }

            ITypeDefinition toAdd = t;
            bool matches = MatchesSuppressM(t);
            if (matches)
            {
                ITypeDefinition stubType = Stubs.GetStubType(t);
                if (stubType != null) toAdd = stubType;
            }
            if (!rtaAnalyzer.visitedClasses.Contains(toAdd) && !rtaAnalyzer.classes.Contains(toAdd))
            {
                rtaAnalyzer.rtaLogSW.WriteLine("SRK_DBG: Adding class: {0}", toAdd.FullName());
                rtaAnalyzer.classes.Add(toAdd);
                rtaAnalyzer.classWorkList.Add(toAdd);
            }
            return toAdd;
        }

        // This method is used during FactGeneration. The above CheckAndAdd methods are used during RTAAnalysis.
        // Since we are not modifying invoke statments to invoke methods in stubs (whenever stubs are used), we need the below method.
        public static IMethodDefinition GetMethodToAnalyze(IMethodDefinition m)
        {
            // Three cases: m should completely be ignored, should be analyzed as is, or the equivalent stub should be analyzed.
            IMethodDefinition methToAnalyze = m;
            bool matches = MatchesSuppressM(m);
            if (matches)
            {
                ITypeDefinition containingType = m.ContainingTypeDefinition;
                containingType = Stubs.GetStubType(containingType);
                if (containingType == null) return null; // This entire containingType is to be ignored.
                if (methToAnalyze is IGenericMethodInstance) methToAnalyze = GenericMethods.GetTemplate(m);
                methToAnalyze = Utils.GetMethodSignMatch(containingType, methToAnalyze);
                if (methToAnalyze == null) return null; // containingType itself is stubbed, but the stub does not define a method equivalent to m.
                if (methToAnalyze.IsGeneric) methToAnalyze = GenericMethods.GetInstantiatedMeth(methToAnalyze, m);
            }
            return methToAnalyze;
        }

        public static ITypeDefinition GetTypeToAnalyze(ITypeDefinition ty)
        {
            ITypeDefinition retTy = ty;
            bool matches = MatchesSuppressM(ty);
            if (matches)
            {
                ITypeDefinition stubType = Stubs.GetStubType(ty);
                if (stubType != null)
                    retTy = stubType;
                else
                    retTy = null;
            }
            return retTy;
        }
    }
}
