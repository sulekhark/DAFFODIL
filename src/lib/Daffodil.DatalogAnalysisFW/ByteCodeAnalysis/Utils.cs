// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Cci;
using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Model;
using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.ThreeAddressCode.Instructions;

namespace Daffodil.DatalogAnalysisFW.AnalysisNetConsole
{
    public static class Utils
    {
        static TypeDefinitionComparer tdc;

        static Utils()
        {
            tdc = new TypeDefinitionComparer();
        }

        public static bool ImplementsInterface(ITypeDefinition tgt, ITypeDefinition queryItf)
        {
            if (queryItf == null || tgt == null) return false;
            
            foreach (ITypeReference itf in tgt.Interfaces)
            {
                ITypeDefinition analItf = Stubber.GetTypeToAnalyze(itf.ResolvedType);
                if (analItf != null && tdc.Equals(queryItf, analItf.ResolvedType)) return true;
            }
            foreach (ITypeReference itf in tgt.Interfaces)
            {
                ITypeReference analItf = Stubber.GetTypeToAnalyze(itf.ResolvedType);
                if (analItf != null && ImplementsInterface(analItf.ResolvedType, queryItf)) return true;
            }
            foreach (ITypeReference bcl in tgt.BaseClasses)
            {
                ITypeReference analBcl = Stubber.GetTypeToAnalyze(bcl.ResolvedType);
                if (analBcl != null && ImplementsInterface(analBcl.ResolvedType, queryItf)) return true;
            }
            return false;
        }

        public static bool ExtendsClass(ITypeDefinition derivedCl, ITypeDefinition baseCl)
        {
            if (derivedCl == null || baseCl == null) return false;
            ITypeDefinition analBaseCl = Stubber.GetTypeToAnalyze(baseCl);
            if (analBaseCl != null && tdc.Equals(derivedCl, analBaseCl)) return true;

            foreach (ITypeReference bcl in derivedCl.BaseClasses)
            {
                ITypeDefinition analBcl = Stubber.GetTypeToAnalyze(bcl.ResolvedType);
                if (analBcl != null && tdc.Equals(baseCl, analBcl)) return true;
            }
            foreach (ITypeReference bcl in derivedCl.BaseClasses)
            {
                ITypeDefinition analBcl = Stubber.GetTypeToAnalyze(bcl.ResolvedType);
                if (ExtendsClass(analBcl, baseCl)) return true;
            }
            return false;
        }

        public static string FullName(this ITypeReference tref)
        {
            return TypeHelper.GetTypeName(tref, NameFormattingOptions.Signature | NameFormattingOptions.TypeParameters);
        }
        public static string GetName(this ITypeReference tref)
        {
            if (tref is INamedTypeReference)
                return (tref as INamedTypeReference).Name.Value;

            return TypeHelper.GetTypeName(tref, NameFormattingOptions.OmitContainingType | 
                NameFormattingOptions.OmitContainingNamespace | NameFormattingOptions.SmartTypeName);
        }

        public static string FullName(this IMethodReference mref)
        {
            return MemberHelper.GetMethodSignature(mref, NameFormattingOptions.Signature | NameFormattingOptions.OmitWhiteSpaceAfterListDelimiter
               | NameFormattingOptions.TypeParameters | NameFormattingOptions.PreserveSpecialNames | NameFormattingOptions.ParameterModifiers);
        }

        public static bool NameMatch(IMethodReference m1, IMethodReference m2)
        {
            string name1 = m1.Name.Value;
            string name2 = m2.Name.Value;
            string extName1 = "." + name1;
            string extName2 = "." + name2;
            return (name1 == name2 || name1.EndsWith(extName2) || name2.EndsWith(extName1));
        }

        public static bool TypeMatch(ITypeReference t1, ITypeReference t2)
        {
            ITypeDefinition tdef1 = t1.ResolvedType;
            ITypeDefinition tdef2 = t2.ResolvedType;
            if (tdc.Equals(tdef1, tdef2)) return true;
           
            if (Stubber.MatchesSuppressM(tdef1)) tdef1 = Stubs.GetStubType(tdef1);
            if (Stubber.MatchesSuppressM(tdef2)) tdef2 = Stubs.GetStubType(tdef2);
            // return (tdef1 != null && tdef2 != null && tdc.Equals(tdef1, tdef2));
            if (tdef1 != null && tdef2 != null)
            {
                if (tdef1 is IGenericTypeInstance)
                    tdef1 = (tdef1 as IGenericTypeInstance).GenericType.ResolvedType;
                if (tdef2 is IGenericTypeInstance)
                    tdef2 = (tdef2 as IGenericTypeInstance).GenericType.ResolvedType;
                return tdc.Equals(tdef1, tdef2);
            }
            else return false;
        }

        public static bool StubbedSignaturesAreEqual(IMethodReference m1, IMethodReference m2)
        {
            bool signEq = false;
            if (m1.ParameterCount == m2.ParameterCount)
            {
                // first check return value type.
                bool retTypeMatch = TypeMatch(m1.Type, m2.Type);
                //If ret value type matches, then check parameter types
                if (retTypeMatch)
                {
                    IList<IParameterTypeInformation> argsList1 = m1.Parameters.ToList();
                    IList<IParameterTypeInformation> argsList2 = m2.Parameters.ToList();
                    bool argTypeMatch = true;
                    for (int i = 0; i < m1.ParameterCount; i++)
                    {
                        if (!TypeMatch(argsList1[i].Type, argsList2[i].Type)) argTypeMatch = false;
                    }
                    if (argTypeMatch) signEq = true;
                }
            }
            return signEq;
        }

        public static bool StubbedGenericMethodSignaturesAreEqual(IMethodReference m1, IMethodReference m2)
        {
            bool signEq;
            signEq = (m1.GenericParameterCount == m2.GenericParameterCount);
            if (signEq) signEq = StubbedSignaturesAreEqual(m1, m2);
            return signEq;
        }

        public static bool MethodSignMatch(IMethodReference m1, IMethodReference m2)
        {
            bool signEq = false;
            if (NameMatch(m1, m2))
            {
                if (!m1.IsGeneric && !m2.IsGeneric)
                {
                    signEq = MemberHelper.SignaturesAreEqual(m1, m2);
                    if (!signEq) signEq = StubbedSignaturesAreEqual(m1, m2);
                }
                else if (m1.IsGeneric && m2.IsGeneric)
                {
                    signEq = MemberHelper.GenericMethodSignaturesAreEqual(m1, m2);
                    if (!signEq) signEq = StubbedGenericMethodSignaturesAreEqual(m1, m2);
                }
            }
            return signEq;
        }

        public static IMethodDefinition GetMethodSignMatch(ITypeDefinition ty, IMethodDefinition meth)
        {
            foreach (IMethodDefinition tyMeth in ty.Methods)
            {
                if (MethodSignMatch(meth, tyMeth)) return tyMeth;
            }
            return null;
        }

        public static IMethodDefinition GetMethodSignMatchRecursive(ITypeDefinition ty, IMethodDefinition meth)
        {
            IMethodDefinition signMatchMeth = GetMethodSignMatch(ty, meth);
            if (signMatchMeth != null) return signMatchMeth;

            foreach (ITypeReference bty in ty.BaseClasses)
            {
                ITypeDefinition analBty = Stubber.GetTypeToAnalyze(bty.ResolvedType);
                if (analBty != null && !Stubber.SuppressF(analBty)) return GetMethodSignMatchRecursive(analBty, meth);
            }
            return null;
        }

        public static IMethodDefinition GetMethodByName(ITypeDefinition ty, string methName)
        {
            foreach (IMethodDefinition tyMeth in ty.Methods)
            {
                if (tyMeth.Name.Value == methName) return tyMeth;
            }
            return null;
        }

        // If P is some property of a class, FullName (which uses CCI's GetMethodSignature) returns P.get whereas internally
        // the name of the method is get_P (ILSpy also shows the name as get_P). 
        // Should be aware of this while using string compare for method names.
        public static IMethodDefinition GetMethodByFullName(ITypeDefinition ty, string methName)
        {
            foreach (IMethodDefinition tyMeth in ty.Methods)
            {
                if (tyMeth.FullName() == methName) return tyMeth;
            }
            return null;
        }

        public static bool IsMainMethod (IMethodDefinition meth)
        {
            if (meth.Name.Value == "Main" && meth.IsStatic) return true;     
            return false;
        }

        public static IMethodDefinition GetStaticConstructor(ITypeDefinition ty)
        {
            foreach (IMethodDefinition meth in ty.Methods)
            {
                if (meth.IsStaticConstructor) return meth;
            }
            return null;
        }

        public static IList<CFGNode> getProgramFlowOrder(ControlFlowGraph cfg)
        {
            IList<CFGNode> cfgNodeList = new List<CFGNode>();
            IDictionary<int, CFGNode> addrToNodeMap = new Dictionary<int, CFGNode>();

            foreach (var node in cfg.Nodes)
            {
                if (node.Instructions.Count > 0)
                {
                    Instruction firstNonPhiInst = null;
                    foreach (Instruction inst in node.Instructions)
                    {
                        if (inst is PhiInstruction) continue;
                        firstNonPhiInst = inst;
                        break;
                    }
                    string lbl = firstNonPhiInst.Label;
                    string pcStr = lbl.Substring(2);
                    int pcVal = Int32.Parse(pcStr, System.Globalization.NumberStyles.HexNumber);
                    addrToNodeMap.Add(pcVal, node);
                }
            }
            int[] addrs = addrToNodeMap.Keys.ToArray();
            Array.Sort(addrs);
            for (int i = 0; i < addrs.Count(); i++)
            {
                cfgNodeList.Add(addrToNodeMap[addrs[i]]);
            }
            return cfgNodeList;
        }
    }
}
