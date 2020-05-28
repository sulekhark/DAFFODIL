// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci;
using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Model;
using Daffodil.DatalogAnalysisFW.AnalysisNetBackend;
using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;
using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.ThreeAddressCode.Values;
using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.ThreeAddressCode.Instructions;
using Daffodil.DatalogAnalysisFW.ProgramFacts;
using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.ThreeAddressCode;
using Daffodil.DatalogAnalysisFW.Common;

namespace Daffodil.DatalogAnalysisFW.AnalysisNetConsole
{
    public class FactGenerator
    {
        public ISet<ITypeDefinition> classes;
        public ISet<ITypeDefinition> types;
        public ISet<IMethodDefinition> methods;
        public ISet<ITypeDefinition> allocClasses;
        public ISet<IMethodDefinition> entryPtMethods;
        public ISet<IFieldDefinition> addrTakenInstFlds;
        public ISet<IFieldDefinition> addrTakenStatFlds;
        public ISet<IVariable> addrTakenLocals;
        public ISet<IMethodDefinition> addrTakenMethods;

        public StreamWriter tacLogSW;
        public StreamWriter factGenLogSW;

        // To be cleared for each method.
        private IVariable currentCatchVar;
        private ExHandlerWrapper prevEhW;


        private ISet<ITypeDefinition> exceptionTypes = new HashSet<ITypeDefinition>();


        public FactGenerator(StreamWriter sw1, StreamWriter sw2)
        {
            tacLogSW = sw1;
            factGenLogSW = sw2;
            //Create a hypothetical field that represents all array elements
            FieldRefWrapper nullFieldRefW = new FieldRefWrapper(null);
            ProgramDoms.domF.Add(nullFieldRefW);
        }

        public void GenerateFacts(MethodBody mBody, ControlFlowGraph cfg, IList<CatchExceptionHandler> ehInfoList)
        {
            // Initialize exception handling book-keeping variables for each analyzed method.
            currentCatchVar = null;
            prevEhW = null;

            IMethodDefinition methDef = mBody.MethodDefinition;
            MethodRefWrapper mRefW = WrapperProvider.getMethodRefW(methDef, mBody);
            tacLogSW.WriteLine();
            tacLogSW.WriteLine(methDef.FullName());
            tacLogSW.WriteLine("==========================================");
            foreach (CatchExceptionHandler eh in ehInfoList)
            {
                tacLogSW.WriteLine(eh.ToString());
            }
            tacLogSW.WriteLine("==========================================");
            ProcessParams(mBody, mRefW);
            ProcessLocals(mBody, mRefW);

            // Going through the instructions via cfg nodes instead of directly iterating over the instructions
            // of the methodBody becuase Phi instructions may not have been inserted in the insts of the methodBody.

            // Program flow order is required for two reasons:
            //    1) To make the dump of the tac code human-readable
            //    2) To get the variable for "rethrow" from the immediately preceding catch block.
            IList<CFGNode> cfgList = Utils.getProgramFlowOrder(cfg);
            foreach (var node in cfgList)
            {
                StringBuilder succStr = new StringBuilder("S: ");
                foreach (CFGNode n in node.Successors) succStr.Append(n.Id + " ");
                StringBuilder predStr = new StringBuilder("P: ");
                foreach (CFGNode n in node.Predecessors) predStr.Append(n.Id + " ");
                tacLogSW.WriteLine("----- BB {0} {1} {2} -----", node.Id, succStr.ToString(), predStr.ToString());
                foreach (var instruction in node.Instructions)
                {
                    tacLogSW.WriteLine("{0}", instruction.ToString());
                    // tacLogSW.WriteLine("{0}", instruction.GetType().ToString());
                    // tacLogSW.WriteLine();
                    InstructionWrapper instW = WrapperProvider.getInstW(instruction, methDef);
                    ProgramDoms.domP.Add(instW);

                    if (instruction is LoadInstruction)
                    {
                        LoadInstruction lInst = instruction as LoadInstruction;
                        ProcessLoadInst(lInst, mRefW);
                    }
                    else if (instruction is StoreInstruction)
                    {
                        StoreInstruction sInst = instruction as StoreInstruction;
                        ProcessStoreInst(sInst, mRefW);
                    }
                    else if (instruction is CreateObjectInstruction)
                    {
                        CreateObjectInstruction newObjInst = instruction as CreateObjectInstruction;
                        ProcessCreateObjectInst(newObjInst, mRefW, methDef);
                    }
                    else if (instruction is CreateArrayInstruction)
                    {
                        CreateArrayInstruction newArrInst = instruction as CreateArrayInstruction;
                        ProcessCreateArrayInst(newArrInst, mRefW, methDef);
                    }
                    else if (instruction is PhiInstruction)
                    {
                        PhiInstruction phiInst = instruction as PhiInstruction;
                        ProcessPhiInst(phiInst, mRefW);
                    }
                    else if (instruction is MethodCallInstruction)
                    {
                        MethodCallInstruction invkInst = instruction as MethodCallInstruction;
                        ProcessMethodCallInst(invkInst, mRefW, instW);
                    }
                    else if (instruction is ConvertInstruction) // cast
                    {
                        ConvertInstruction castInst = instruction as ConvertInstruction;
                        ProcessConvertInst(castInst, mRefW);
                    }
                    else if (instruction is ReturnInstruction)
                    {
                        ReturnInstruction retInst = instruction as ReturnInstruction;
                        ProcessRetInst(retInst, mRefW);
                    }
                    else if (instruction is ThrowInstruction)
                    {
                        ThrowInstruction throwInst = instruction as ThrowInstruction;
                        ProcessThrowInst(throwInst, instW, mRefW);
                    }
                    else if (instruction is CatchInstruction)
                    {
                        CatchInstruction catchInst = instruction as CatchInstruction;
                        ProcessCatchInst(catchInst, mRefW, methDef);
                    }
                    else if (instruction is InitializeObjectInstruction)
                    {
                        // Ignore
                    }
                    else if (instruction is LoadTokenInstruction)
                    {
                        // Ignore
                    }
                    else if (instruction is BranchInstruction)
                    {
                        // Ignore
                    }
                    else if (instruction is UnaryInstruction || instruction is BinaryInstruction || instruction is NopInstruction)
                    {
                        // Ignore
                    }

                    else if (instruction is BreakpointInstruction || instruction is TryInstruction ||
                             instruction is FaultInstruction || instruction is FinallyInstruction)
                    {
                        // Ignore
                    }
                    else if (instruction is SwitchInstruction || instruction is SizeofInstruction)
                    {
                        // Ignore
                    }
                    else
                    {
                        // System.Console.WriteLine("{0}", instruction.ToString());
                        // System.Console.WriteLine("Not currently handled: {0}", instruction.GetType().ToString());
                        // System.Console.WriteLine();
                    }
                }
            }
            ComputeExceptionRanges(cfgList, methDef);
        }

        public void GenerateTypeAndMethodFacts()
        {
            foreach (IMethodDefinition meth in methods)
            {
                MethodRefWrapper methW = WrapperProvider.getMethodRefW(meth);
                ProgramDoms.domM.Add(methW);
                if (addrTakenMethods.Contains(meth))
                {
                    AddressWrapper mAddrW = WrapperProvider.getAddrW(meth);
                    ProgramDoms.domX.Add(mAddrW);
                    ProgramRels.relAddrOfMX.Add(methW, mAddrW);
                }
                if (meth.IsStatic)
                {
                    ITypeDefinition cl = meth.ContainingTypeDefinition;
                    TypeRefWrapper clW = WrapperProvider.getTypeRefW(cl);
                    ProgramDoms.domT.Add(clW);
                    ProgramRels.relStaticTM.Add(clW, methW);
                }
                if (meth.IsStaticConstructor)
                {
                    ITypeDefinition cl = meth.ContainingTypeDefinition;
                    TypeRefWrapper clW = WrapperProvider.getTypeRefW(cl);
                    ProgramDoms.domT.Add(clW);
                    ProgramRels.relClinitTM.Add(clW, methW);
                }
            }
            foreach (IMethodDefinition meth in entryPtMethods)
            {
                MethodRefWrapper methW = WrapperProvider.getMethodRefW(meth);
                ProgramRels.relEntryPtM.Add(methW);
            }
            foreach (ITypeDefinition ty in classes)
            {
                TypeRefWrapper tyW = WrapperProvider.getTypeRefW(ty);
                ProgramDoms.domT.Add(tyW);
                if (!ty.IsInterface)
                {
                    ProgramRels.relClassT.Add(tyW);
                }
                foreach (IFieldDefinition fld in ty.Fields)
                {
                    FieldRefWrapper fldW = WrapperProvider.getFieldRefW(fld);
                    ITypeDefinition fldType = fld.Type.ResolvedType;
                    if ((fldType.IsValueType && !fldType.IsStruct) || Stubber.SuppressF(fldType)) continue;
                    TypeRefWrapper fldTypeRefW = WrapperProvider.getTypeRefW(fldType);
                    ProgramDoms.domT.Add(fldTypeRefW);
                    ProgramDoms.domF.Add(fldW);
                    ProgramRels.relFT.Add(fldW, fldTypeRefW);
                    if (fld.IsStatic)
                    {
                        ProgramRels.relStaticTF.Add(tyW, fldW);
                        if (addrTakenStatFlds.Contains(fld))
                        {
                            AddressWrapper fldAddrW = WrapperProvider.getAddrW(fld);
                            ProgramDoms.domX.Add(fldAddrW);
                            ProgramRels.relAddrOfFX.Add(fldW, fldAddrW);
                        }
                        if (fldType.IsValueType && fldType.IsStruct)
                        {
                            // Even though structs are value types, in our memory model, we allocate them on the heap.
                            CreateStruct(ty, fld);
                        }
                    }
                }
            }
        }

        public void CheckDomX()
        {
            if (ProgramDoms.domX.Count() == 0)
            {
                IMethodDefinition someMeth = methods.ElementAt(0);
                MethodRefWrapper methW = WrapperProvider.getMethodRefW(someMeth);
                AddressWrapper mAddrW = WrapperProvider.getAddrW(someMeth);
                ProgramDoms.domX.Add(mAddrW);
                ProgramRels.relAddrOfMX.Add(methW, mAddrW);
            }
        }

        public void GenerateChaFacts()
        {
            ClassHierarchyAnalysis cha = new ClassHierarchyAnalysis();
            cha.Analyze(classes);
            foreach (ITypeReference ty in cha.Types)
            {
                TypeRefWrapper tyW = WrapperProvider.getTypeRefW(ty);
                ProgramRels.relSub.Add(tyW, tyW);
                foreach (ITypeDefinition subTy in cha.GetSubtypes(ty))
                {
                    TypeRefWrapper subTyW = WrapperProvider.getTypeRefW(subTy);
                    ProgramRels.relSub.Add(subTyW, tyW);
                }

                if (ty.FullName().StartsWith("System.Exception"))
                {
                    ProgramRels.relExceptionType.Add(tyW);
                    exceptionTypes.Add(ty.ResolvedType);
                    foreach (ITypeDefinition subTy in cha.GetAllSubtypes(ty))
                    {
                        exceptionTypes.Add(subTy);
                        TypeRefWrapper subTyW = WrapperProvider.getTypeRefW(subTy);
                        ProgramRels.relExceptionType.Add(subTyW);
                    }
                }
            }

            ISet<IMethodDefinition> allInstanceMethods = new HashSet<IMethodDefinition>();
            foreach (IMethodDefinition meth in methods)
            {
                if (!meth.IsStatic) allInstanceMethods.Add(meth);
            }
            foreach (ITypeDefinition ty in classes)
            {
                TypeRefWrapper tyW = WrapperProvider.getTypeRefW(ty);
                if (ty is IArrayTypeReference)
                {
                    foreach (IMethodDefinition instMeth in allInstanceMethods)
                    {
                        MethodRefWrapper instMethW = WrapperProvider.getMethodRefW(instMeth);
                        ProgramRels.relCha.Add(instMethW, tyW, instMethW);
                    }
                    continue;
                }
                else
                {
                    bool tyIsInterface = false;
                    if (ty.IsInterface) tyIsInterface = true;

                    foreach (IMethodDefinition tyMeth in ty.Methods)
                    {
                        if (!tyMeth.IsGeneric && !methods.Contains(tyMeth)) continue;
                        IList<IMethodDefinition> instantiatedMeths;
                        if (tyMeth.IsGeneric && ClassAndMethodVisitor.genericMethodMap.ContainsKey(tyMeth))
                        {
                            instantiatedMeths = ClassAndMethodVisitor.genericMethodMap[tyMeth].Values.ToList();
                        }
                        else if (!tyMeth.IsGeneric)
                        {
                            instantiatedMeths = new List<IMethodDefinition>();
                            instantiatedMeths.Add(tyMeth);
                        }
                        else continue;
                        if (tyMeth.Visibility == TypeMemberVisibility.Private || tyMeth.IsConstructor)
                        {
                            foreach (IMethodDefinition meth in instantiatedMeths) InsertIntoCha(meth, ty, meth);
                            continue; // No need to check for dynamic dispatch possibilities.
                        }
                        foreach (IMethodDefinition meth in instantiatedMeths)
                        {
                            IMethodDefinition methArg;
                            methArg = (meth is IGenericMethodInstance && tyMeth.IsGeneric) ? tyMeth : meth;
                            foreach (ITypeDefinition candidateTy in allocClasses)
                            {
                                if (candidateTy is IArrayTypeReference) continue;
                                IMethodDefinition candidateMeth = null;
                                if (tyIsInterface)
                                {
                                    if (Utils.ImplementsInterface(candidateTy, ty)) 
                                        candidateMeth = Utils.GetMethodSignMatchRecursive(candidateTy, methArg);
                                }
                                else
                                {
                                    if (Utils.ExtendsClass(candidateTy, ty))
                                    {
                                        candidateMeth = Utils.GetMethodSignMatchRecursive(candidateTy, methArg);
                                        if (candidateMeth.IsAbstract || candidateMeth.IsExternal) candidateMeth = null;
                                    }
                                }
                                if (candidateMeth != null) InsertIntoCha(meth, candidateTy, candidateMeth);
                            }
                        }
                    }
                }
            }
        }

        void InsertIntoCha(IMethodDefinition meth, ITypeDefinition candidateTy, IMethodDefinition candidateMeth)
        {
            if (meth is IGenericMethodInstance && candidateMeth.IsGeneric)
            {
                if (candidateMeth != null &&
                    ClassAndMethodVisitor.genericMethodMap.ContainsKey(candidateMeth))
                {
                    IDictionary<string, IMethodDefinition> candidateInsts =
                        ClassAndMethodVisitor.genericMethodMap[candidateMeth];
                    string genericArgsStr =
                        GenericMethods.GetGenericArgStr((meth as IGenericMethodInstance).GenericArguments);
                    if (candidateInsts.ContainsKey(genericArgsStr))
                        candidateMeth = candidateInsts[genericArgsStr];
                    else
                        candidateMeth = null;
                }
            }
            if (candidateMeth != null && methods.Contains(candidateMeth))
            {
                MethodRefWrapper methW = WrapperProvider.getMethodRefW(meth);
                MethodRefWrapper candidateMethW = WrapperProvider.getMethodRefW(candidateMeth);
                TypeRefWrapper candidateTyW = WrapperProvider.getTypeRefW(candidateTy);
                ProgramRels.relCha.Add(methW, candidateTyW, candidateMethW);
            }
        }

        void ProcessParams(MethodBody mBody, MethodRefWrapper mRefW)
        {
            IList<IVariable> paramList = mBody.Parameters;
            int paramNdx = 0;
            foreach (IVariable param in paramList)
            {
                if (Stubber.SuppressF(param.Type.ResolvedType)) continue;
                if (!param.Type.IsValueType || param.Type.ResolvedType.IsStruct)
                {
                    VariableWrapper paramW = WrapperProvider.getVarW(param);
                    ProgramDoms.domV.Add(paramW);
                    ProgramRels.relMV.Add(mRefW, paramW);
                    ProgramRels.relMmethArg.Add(mRefW, paramNdx, paramW);
                    ITypeReference varTypeRef = param.Type;
                    TypeRefWrapper varTypeRefW = WrapperProvider.getTypeRefW(varTypeRef.ResolvedType);
                    ProgramDoms.domT.Add(varTypeRefW);
                    bool success = ProgramRels.relVT.Add(paramW, varTypeRefW);
                    if (param.Type.ResolvedType.IsStruct && param.Type.IsValueType) CreateStruct(mRefW, param);
                    if (addrTakenLocals.Contains(param))
                    {
                        AddressWrapper varAddrW = WrapperProvider.getAddrW(param);
                        ProgramDoms.domX.Add(varAddrW);
                        ProgramRels.relAddrOfVX.Add(paramW, varAddrW);
                    }
                }
                paramNdx++;
            }
            return;
        }

        void ProcessLocals(MethodBody mBody, MethodRefWrapper mRefW)
        {
            ISet<IVariable> localVarSet = mBody.Variables;
            foreach (IVariable lclVar in localVarSet)
            {
                if (!lclVar.IsParameter) // Parameters are processed in ProcessParams
                {
                    if (Stubber.SuppressF(lclVar.Type.ResolvedType)) continue;
                    if (!lclVar.Type.IsValueType || lclVar.Type.ResolvedType.IsStruct)
                    {
                        VariableWrapper lclW = WrapperProvider.getVarW(lclVar);
                        ProgramDoms.domV.Add(lclW);
                        ProgramRels.relMV.Add(mRefW, lclW);
                        ITypeReference varTypeRef = lclVar.Type;
                        TypeRefWrapper varTypeRefW = WrapperProvider.getTypeRefW(varTypeRef.ResolvedType);
                        ProgramDoms.domT.Add(varTypeRefW);
                        ProgramRels.relVT.Add(lclW, varTypeRefW);
                        if (lclVar.Type.ResolvedType.IsStruct && lclVar.Type.IsValueType)
                        {
                            CreateStruct(mRefW, lclVar);
                        }
                        else if (lclVar.Type.TypeCode == PrimitiveTypeCode.Reference)
                        {
                            if ((lclVar.Type as IManagedPointerTypeReference).TargetType.ResolvedType.IsStruct)
                            {
                                ProgramRels.relStructRefV.Add(lclW);
                            }
                        }
                        if (addrTakenLocals.Contains(lclVar))
                        {
                            AddressWrapper varAddrW = WrapperProvider.getAddrW(lclVar);
                            ProgramDoms.domX.Add(varAddrW);
                            ProgramRels.relAddrOfVX.Add(lclW, varAddrW);
                        }
                    }
                }
            }
            return;
        }

        void ProcessLoadInst(LoadInstruction lInst, MethodRefWrapper mRefW)
        {
            if (Stubber.SuppressF(lInst.Result.Type.ResolvedType))
                return;
            else if (!lInst.Result.Type.IsValueType || lInst.Result.Type.ResolvedType.IsStruct)
            {
                ProcessLoad(lInst, mRefW);
            }
            return;
        }

        void ProcessLoad(LoadInstruction lInst, MethodRefWrapper mRefW)
        {
            IVariable lhsVar = lInst.Result;
            VariableWrapper lhsW = WrapperProvider.getVarW(lhsVar);
            IValue rhsOperand = lInst.Operand;
            if (rhsOperand is IVariable)
            {
                IVariable rhsVar = rhsOperand as IVariable;
                VariableWrapper rhsW = WrapperProvider.getVarW(rhsVar);
                bool success = ProgramRels.relMMove.Add(mRefW, lhsW, rhsW);
            }
            else if (rhsOperand is InstanceFieldAccess)
            {
                InstanceFieldAccess rhsAcc = rhsOperand as InstanceFieldAccess;
                IVariable rhsVar = rhsAcc.Instance;
                VariableWrapper rhsW = WrapperProvider.getVarW(rhsVar);
                IFieldDefinition fld = rhsAcc.Field.ResolvedField;
                FieldRefWrapper fldW = WrapperProvider.getFieldRefW(fld);
                bool success = ProgramRels.relMInstFldRead.Add(mRefW, lhsW, rhsW, fldW);
            }
            else if (rhsOperand is StaticFieldAccess)
            {
                StaticFieldAccess rhsAcc = rhsOperand as StaticFieldAccess;
                IFieldDefinition fld = rhsAcc.Field.ResolvedField;
                FieldRefWrapper fldW = WrapperProvider.getFieldRefW(fld);
                bool success = ProgramRels.relMStatFldRead.Add(mRefW, lhsW, fldW);
            }
            else if (rhsOperand is ArrayElementAccess)
            {
                ArrayElementAccess rhsArr = rhsOperand as ArrayElementAccess;
                IVariable arr = rhsArr.Array;
                VariableWrapper arrW = WrapperProvider.getVarW(arr);
                FieldRefWrapper arrElemRepW = ProgramDoms.domF.GetVal(0);
                bool success = ProgramRels.relMInstFldRead.Add(mRefW, lhsW, arrW, arrElemRepW);
            }
            // Note: calls to static methods and instance methods appear as a StaticMethodReference
            else if (rhsOperand is StaticMethodReference)
            {
                StaticMethodReference sMethAddr = rhsOperand as StaticMethodReference;
                IMethodDefinition tgtMeth = sMethAddr.Method.ResolvedMethod;
                MethodRefWrapper tgtMethW = WrapperProvider.getMethodRefW(tgtMeth);
                ProgramRels.relMAddrTakenFunc.Add(mRefW, lhsW, tgtMethW);
            }
            //Note: calls to virtual, abstract or interface methods appear as VirtualMethodReference
            else if (rhsOperand is VirtualMethodReference)
            {
                VirtualMethodReference sMethAddr = rhsOperand as VirtualMethodReference;
                IMethodDefinition tgtMeth = sMethAddr.Method.ResolvedMethod;
                MethodRefWrapper tgtMethW = WrapperProvider.getMethodRefW(tgtMeth);
                ProgramRels.relMAddrTakenFunc.Add(mRefW, lhsW, tgtMethW);
            }
            else if (rhsOperand is Dereference)
            {
                Dereference rhsDeref = rhsOperand as Dereference;
                VariableWrapper rhsW = WrapperProvider.getVarW(rhsDeref.Reference);
                ProgramRels.relMDerefRight.Add(mRefW, lhsW, rhsW);
            }
            else if (rhsOperand is Reference)
            {
                Reference rhsRef = rhsOperand as Reference;
                IReferenceable refOf = rhsRef.Value;
                if (refOf is IVariable)
                {
                    IVariable refVar = refOf as IVariable;
                    VariableWrapper refW = WrapperProvider.getVarW(refVar);
                    ProgramRels.relMAddrTakenLocal.Add(mRefW, lhsW, refW);
                }
                else if (refOf is StaticFieldAccess)
                {
                    StaticFieldAccess refAcc = refOf as StaticFieldAccess;
                    IFieldDefinition fld = refAcc.Field.ResolvedField;
                    FieldRefWrapper fldW = WrapperProvider.getFieldRefW(fld);
                    ProgramRels.relMAddrTakenStatFld.Add(mRefW, lhsW, fldW);
                }
                else if (refOf is InstanceFieldAccess)
                {
                    InstanceFieldAccess refAcc = refOf as InstanceFieldAccess;
                    IVariable refVar = refAcc.Instance;
                    VariableWrapper refW = WrapperProvider.getVarW(refVar);
                    IFieldDefinition fld = refAcc.Field.ResolvedField;
                    FieldRefWrapper fldW = WrapperProvider.getFieldRefW(fld);
                    ProgramRels.relMAddrTakenInstFld.Add(mRefW, lhsW, refW, fldW);
                }
                else if (refOf is ArrayElementAccess)
                {
                    ArrayElementAccess refArr = refOf as ArrayElementAccess;
                    IVariable arr = refArr.Array;
                    VariableWrapper arrW = WrapperProvider.getVarW(arr);
                    FieldRefWrapper arrElemRepW = ProgramDoms.domF.GetVal(0);
                    ProgramRels.relMAddrTakenInstFld.Add(mRefW, lhsW, arrW, arrElemRepW);
                }
            }
            else if (rhsOperand is Constant)
            {
                // System.Console.WriteLine("WARNING: unhandled: Load Constant");
            }
            else if (rhsOperand is ArrayLengthAccess)
            {
                // System.Console.WriteLine("WARNING: unhandled: Load ArrayLengthAccess");
            }
            else
            {
                System.Console.WriteLine("WARNING: unhandled: Load Inst: {0}   {1}", rhsOperand.GetType(), rhsOperand.ToString());
            }
            return;
        }

        void ProcessStoreInst(StoreInstruction sInst, MethodRefWrapper mRefW)
        {
            if (Stubber.SuppressF(sInst.Operand.Type.ResolvedType))
                return;
            else if (!sInst.Operand.Type.IsValueType || sInst.Operand.Type.ResolvedType.IsStruct)
            {
                ProcessStore(sInst, mRefW);
            }
            return;
        }

        void ProcessStore(StoreInstruction sInst, MethodRefWrapper mRefW)
        {
            IVariable rhsVar = sInst.Operand;
            VariableWrapper rhsW = WrapperProvider.getVarW(rhsVar);
            IAssignableValue lhs = sInst.Result;
            if (lhs is InstanceFieldAccess)
            {
                InstanceFieldAccess lhsAcc = lhs as InstanceFieldAccess;
                IVariable lhsVar = lhsAcc.Instance;
                VariableWrapper lhsW = WrapperProvider.getVarW(lhsVar);
                IFieldDefinition fld = lhsAcc.Field.ResolvedField;
                FieldRefWrapper fldW = WrapperProvider.getFieldRefW(fld);
                bool success = ProgramRels.relMInstFldWrite.Add(mRefW, lhsW, fldW, rhsW);
            }
            else if (lhs is StaticFieldAccess)
            {
                StaticFieldAccess lhsAcc = lhs as StaticFieldAccess;
                IFieldDefinition fld = lhsAcc.Field.ResolvedField;
                FieldRefWrapper fldW = WrapperProvider.getFieldRefW(fld);
                bool success = ProgramRels.relMStatFldWrite.Add(mRefW, fldW, rhsW);
            }
            else if (lhs is ArrayElementAccess)
            {
                ArrayElementAccess lhsArr = lhs as ArrayElementAccess;
                IVariable arr = lhsArr.Array;
                VariableWrapper arrW = WrapperProvider.getVarW(arr);
                FieldRefWrapper arrElemRepW = ProgramDoms.domF.GetVal(0);
                bool success = ProgramRels.relMInstFldWrite.Add(mRefW, arrW, arrElemRepW, rhsW);
            }
            else if (lhs is Dereference)
            {
                Dereference lhsDeref = lhs as Dereference;
                VariableWrapper lhsW = WrapperProvider.getVarW(lhsDeref.Reference);
                ProgramRels.relMDerefLeft.Add(mRefW, lhsW, rhsW);
            }
            else
            {
                System.Console.WriteLine("WARNING: unhandled: Store Inst: {0}   {1}", lhs.GetType(), lhs.ToString());
            }
            return;
        }

        void ProcessCreateObjectInst(CreateObjectInstruction newObjInst, MethodRefWrapper mRefW, IMethodDefinition methDef)
        {
            IVariable lhsVar = newObjInst.Result;
            ITypeDefinition objTypeDef = newObjInst.AllocationType.ResolvedType;
            if (Stubber.SuppressF(objTypeDef)) return;
            if (ConfigParams.SuppressSystemExceptions)
            {
                if (exceptionTypes.Contains(objTypeDef) && methDef.FullName().StartsWith("System.")) return;
            }
           
            VariableWrapper lhsW = WrapperProvider.getVarW(lhsVar);
            TypeRefWrapper objTypeW = WrapperProvider.getTypeRefW(objTypeDef);
            HeapElemWrapper hpW = WrapperProvider.getHeapElemW(newObjInst, methDef, false);
            ProgramDoms.domH.Add(hpW);
            ProgramRels.relMAlloc.Add(mRefW, lhsW, hpW);
            ProgramRels.relHT.Add(hpW, objTypeW);
            // Note that lhsVar is a reference to a struct. Here the struct is allocated on the heap.
            if (objTypeDef.IsStruct) ProgramRels.relStructH.Add(hpW);

            IList<ITypeDefinition> workList = new List<ITypeDefinition>();
            workList.Add(objTypeDef);
            while (workList.Count > 0)
            {
                ITypeDefinition currTypeDef = workList[0];
                workList.RemoveAt(0);
                foreach (IFieldDefinition fld in currTypeDef.Fields)
                {
                    if (!fld.IsStatic)
                    {
                        if (addrTakenInstFlds.Contains(fld))
                        {
                            FieldRefWrapper fldW = WrapperProvider.getFieldRefW(fld);
                            AddressWrapper allocfldAddrW = WrapperProvider.getAddrW(newObjInst, fld, methDef);
                            ProgramDoms.domX.Add(allocfldAddrW);
                            ProgramRels.relAddrOfHFX.Add(hpW, fldW, allocfldAddrW);
                        }
                        ITypeDefinition fldType = fld.Type.ResolvedType;
                        if (!Stubber.SuppressF(fldType) && fldType.IsValueType && fldType.IsStruct) CreateStruct(hpW, fld);
                    }
                }
                foreach (ITypeReference baseTypeRef in currTypeDef.BaseClasses) workList.Add(baseTypeRef.ResolvedType);
            }
            return;
        }

        void ProcessCreateArrayInst(CreateArrayInstruction newArrInst, MethodRefWrapper mRefW, IMethodDefinition methDef)
        {
            IVariable lhsVar = newArrInst.Result;
            ITypeDefinition arrTypeDef = lhsVar.Type.ResolvedType;
            if (Stubber.SuppressF(arrTypeDef)) return;

            VariableWrapper lhsW = WrapperProvider.getVarW(lhsVar);
            TypeRefWrapper arrTypeW = WrapperProvider.getTypeRefW(arrTypeDef);
            HeapElemWrapper hpW = WrapperProvider.getHeapElemW(newArrInst, methDef, false);
            ProgramDoms.domH.Add(hpW);
            ProgramRels.relMAlloc.Add(mRefW, lhsW, hpW);
            ProgramRels.relHT.Add(hpW, arrTypeW);

            ITypeDefinition elemTypeDef = newArrInst.ElementType.ResolvedType;
            // Even though structs are value types, in our memory model, we allocate them on the heap.
            if (elemTypeDef.IsStruct)
            {
                HeapElemWrapper hpArrElemW = WrapperProvider.getHeapElemW(newArrInst, methDef, true);
                CreateStruct(mRefW, hpW, elemTypeDef, hpArrElemW);
            }

            // By default, create an entry in domX for the array element as potential address-taken.
            AddressWrapper arrayAddrW = WrapperProvider.getAddrW(newArrInst, methDef);
            ProgramDoms.domX.Add(arrayAddrW);
            FieldRefWrapper fldW = ProgramDoms.domF.GetVal(0);
            ProgramRels.relAddrOfHFX.Add(hpW, fldW, arrayAddrW);
            return;
        }

        void ProcessPhiInst(PhiInstruction phiInst, MethodRefWrapper mRefW)
        {
            IVariable lhsVar = phiInst.Result;
            if (Stubber.SuppressF(lhsVar.Type.ResolvedType)) return;

            if (!lhsVar.Type.IsValueType || lhsVar.Type.ResolvedType.IsStruct)
            {
                VariableWrapper lhsW = WrapperProvider.getVarW(lhsVar);
                ProgramDoms.domV.Add(lhsW);
                ProgramRels.relMV.Add(mRefW, lhsW);
                IList<IVariable> phiArgList = phiInst.Arguments;
                foreach (IVariable arg in phiArgList)
                {
                    VariableWrapper argW = WrapperProvider.getVarW(arg);
                    ProgramDoms.domV.Add(argW);
                    ProgramRels.relMV.Add(mRefW, argW);
                    bool success = ProgramRels.relMMove.Add(mRefW, lhsW, argW);
                }
            }
            return;
        }

        void ProcessMethodCallInst(MethodCallInstruction invkInst, MethodRefWrapper mRefW, InstructionWrapper instW)
        {
            bool done = SpecialHandlingOfInvoke(invkInst, mRefW, instW);
            if (done) return;

            ProgramDoms.domI.Add(instW);
            ProgramDoms.domP.Add(instW);  // At present, for throw/catch processing
            ProgramRels.relMI.Add(mRefW, instW);
            ProgramRels.relPI.Add(instW, instW);

            if (invkInst.HasResult)
            {
                IVariable lhsVar = invkInst.Result;
                ITypeDefinition lhsType = lhsVar.Type.ResolvedType;
                if (!Stubber.SuppressF(lhsType) && (!lhsType.IsValueType || lhsType.IsStruct))
                {
                    VariableWrapper lhsW = WrapperProvider.getVarW(lhsVar);
                    ProgramRels.relIinvkRet.Add(instW, 0, lhsW);
                }
            }
            IMethodDefinition origTgtDef = invkInst.Method.ResolvedMethod;
            IMethodDefinition callTgtDef = Stubber.GetMethodToAnalyze(origTgtDef);
            if (callTgtDef == null) return;
            MethodRefWrapper callTgtW = WrapperProvider.getMethodRefW(callTgtDef);
            ProgramDoms.domM.Add(callTgtW);
            IList<IVariable> invkArgs = invkInst.Arguments;
            if (invkArgs.Count > 0)
            {
                IVariable arg0 = invkArgs[0];
                ITypeDefinition arg0Type = arg0.Type.ResolvedType;
                if (!Stubber.SuppressF(arg0Type) && (!arg0Type.IsValueType || arg0Type.IsStruct))
                {
                    VariableWrapper arg0W = WrapperProvider.getVarW(arg0);
                    ProgramRels.relIinvkArg0.Add(instW, arg0W);
                    if (arg0Type.TypeCode == PrimitiveTypeCode.Reference) ProgramRels.relThisRefV.Add(arg0W);
                }
            }
            int argNdx = 0;
            foreach (IVariable arg in invkArgs)
            {
                ITypeDefinition argType = arg.Type.ResolvedType;
                if (!Stubber.SuppressF(argType) && (!argType.IsValueType || argType.IsStruct))
                {
                    VariableWrapper argW = WrapperProvider.getVarW(arg);
                    ProgramRels.relIinvkArg.Add(instW, argNdx, argW);
                }
                argNdx++;
            }
            MethodCallOperation callType = invkInst.Operation;
            if (callType == MethodCallOperation.Virtual)
            {
                ProgramRels.relVirtIM.Add(instW, callTgtW);
            }
            else if (callType == MethodCallOperation.Static)
            {
                ProgramRels.relStatIM.Add(instW, callTgtW);
            }
            else
            {
                // The only other type is MethodCallOperation.Jump which we ignore.
            }
            return;
        }

        bool SpecialHandlingOfInvoke(MethodCallInstruction invkInst, MethodRefWrapper mRefW, InstructionWrapper instW)
        {
            IMethodReference callTgt = invkInst.Method;
            IMethodDefinition callTgtDef = callTgt.ResolvedMethod;
            ITypeDefinition declType = callTgtDef.ContainingTypeDefinition;

            if (declType.IsDelegate && callTgtDef.IsConstructor)
            {
                IList<IVariable> invkArgs = invkInst.Arguments;
                if (invkArgs.Count != 3) Console.WriteLine("WARNING: Delegate constructor invoke has args different from 3.");
                VariableWrapper delegateVarW = WrapperProvider.getVarW(invkArgs[0]);
                VariableWrapper receiverVarW = WrapperProvider.getVarW(invkArgs[1]);
                VariableWrapper funcPtrVarW = WrapperProvider.getVarW(invkArgs[2]);
                FieldRefWrapper dummyElemW = ProgramDoms.domF.GetVal(0);
                ProgramRels.relMInstFldWrite.Add(mRefW, delegateVarW, dummyElemW, receiverVarW);
                ProgramRels.relMInstFldWrite.Add(mRefW, delegateVarW, dummyElemW, funcPtrVarW);
                return true;
            }
            else if (declType.IsDelegate && callTgt.Name.ToString() == "Invoke")
            {
                ProgramDoms.domI.Add(instW);
                ProgramDoms.domP.Add(instW);
                ProgramRels.relMI.Add(mRefW, instW);
                ProgramRels.relPI.Add(instW, instW);

                if (invkInst.HasResult)
                {
                    IVariable lhsVar = invkInst.Result;
                    ITypeDefinition lhsType = lhsVar.Type.ResolvedType;
                    if (!Stubber.SuppressF(lhsType) && (!lhsType.IsValueType || lhsType.IsStruct))
                    {
                        VariableWrapper lhsW = WrapperProvider.getVarW(lhsVar);
                        ProgramRels.relIinvkRet.Add(instW, 0, lhsW);
                    }
                }
                IList<IVariable> invkArgs = invkInst.Arguments;
                VariableWrapper delegateVarW = WrapperProvider.getVarW(invkArgs[0]);
                ProgramRels.relDelegateIV.Add(instW, delegateVarW);
                int argNdx = 1;
                for (int i = 1; i < invkArgs.Count; i++)
                {
                    IVariable arg = invkArgs[i];
                    ITypeDefinition argType = arg.Type.ResolvedType;
                    if (!Stubber.SuppressF(argType) && (!argType.IsValueType || argType.IsStruct))
                    {
                        VariableWrapper argW = WrapperProvider.getVarW(arg);
                        ProgramRels.relIinvkArg.Add(instW, argNdx, argW);
                    }
                    argNdx++;
                }
                return true;
            }
            else if (declType.FullName().Equals("System.Delegate") && callTgt.Name.ToString() == "Combine")
            {
                if (!invkInst.HasResult) Console.WriteLine("WARNING: Delegate Combine function has no return register.");
                VariableWrapper lhsVarW = WrapperProvider.getVarW(invkInst.Result);
                IList<IVariable> invkArgs = invkInst.Arguments;
                if (invkArgs.Count != 2) Console.WriteLine("WARNING: Delegate Combine has args different from 2.");
                VariableWrapper rhsVarW1 = WrapperProvider.getVarW(invkArgs[0]);
                VariableWrapper rhsVarW2 = WrapperProvider.getVarW(invkArgs[1]);
                ProgramRels.relMMove.Add(mRefW, lhsVarW, rhsVarW1);
                ProgramRels.relMMove.Add(mRefW, lhsVarW, rhsVarW2);
                return true;
            }
            return false;
        }

        void ProcessConvertInst(ConvertInstruction castInst, MethodRefWrapper mRefW)
        {
            IVariable lhsVar = castInst.Result;
            ITypeDefinition lhsType = lhsVar.Type.ResolvedType;
            if (!Stubber.SuppressF(lhsType) && (!lhsType.IsValueType || lhsType.IsStruct))
            {
                VariableWrapper lhsW = WrapperProvider.getVarW(lhsVar);
                IVariable rhsVar = castInst.Operand;
                VariableWrapper rhsW = WrapperProvider.getVarW(rhsVar);
                bool success = ProgramRels.relMMove.Add(mRefW, lhsW, rhsW);
            }
            return;
        }

        void ProcessRetInst(ReturnInstruction retInst, MethodRefWrapper mRefW)
        {
            IVariable retVar = retInst.Operand;
            if (retVar == null || Stubber.SuppressF(retVar.Type.ResolvedType)) return;
            else if (!retVar.Type.IsValueType || retVar.Type.ResolvedType.IsStruct)
            {
                VariableWrapper retW = WrapperProvider.getVarW(retVar);
                ProgramRels.relMmethRet.Add(mRefW, 0, retW);
            }
            return;
        }

        void ProcessThrowInst(ThrowInstruction throwInst, InstructionWrapper instW, MethodRefWrapper mRefW)
        {
            ProgramDoms.domP.Add(instW);
            IVariable throwVar = throwInst.Operand;
            if (throwVar == null) throwVar = currentCatchVar;
            VariableWrapper varW = WrapperProvider.getVarW(throwVar);
            ProgramRels.relThrowPV.Add(mRefW, instW, varW);
            return;
        }

        void ProcessCatchInst(CatchInstruction catchInst, MethodRefWrapper mRefW, IMethodDefinition methDef)
        {
            ExHandlerWrapper currEhW = WrapperProvider.getExHandlerW(catchInst, methDef);
            ProgramDoms.domEH.Add(currEhW);
            ProgramRels.relMEH.Add(mRefW, currEhW);
            IVariable catchVar = catchInst.Result;
            currentCatchVar = catchVar;
            VariableWrapper varW = WrapperProvider.getVarW(catchVar);
            ITypeDefinition catchType = catchVar.Type.ResolvedType;
            TypeRefWrapper typeRefW = WrapperProvider.getTypeRefW(catchType);
            ProgramRels.relVarEH.Add(currEhW, varW);
            ProgramRels.relTypeEH.Add(currEhW, typeRefW);
            if (prevEhW != null) ProgramRels.relPrevEH.Add(prevEhW, currEhW);
            prevEhW = currEhW;
            return;
        }

        void ComputeExceptionRanges(IList<CFGNode> cfgList, IMethodDefinition methDef)
        {
            IDictionary<int, ExHandlerWrapper> nodeidToWrapperMap = new Dictionary<int, ExHandlerWrapper>();
            foreach (CFGNode node in cfgList)
            {
                CatchInstruction catchInst = FindCatchInstruction(node);
                if (catchInst != null)
                {
                    ExHandlerWrapper catchW = WrapperProvider.getExHandlerW(catchInst, methDef);
                    nodeidToWrapperMap[node.Id] = catchW;
                }
            }

            foreach (CFGNode node in cfgList)
            {
                foreach (var instruction in node.Instructions)
                {
                    if (instruction is MethodCallInstruction || instruction is ThrowInstruction)
                    {
                        InstructionWrapper instW = WrapperProvider.getInstW(instruction, methDef);
                        foreach (CFGNode succ in node.Successors)
                        {
                            if (nodeidToWrapperMap.ContainsKey(succ.Id))
                            {
                                ExHandlerWrapper catchW = nodeidToWrapperMap[succ.Id];
                                ProgramRels.relInRange.Add(catchW, instW);
                            }
                        }
                    }
                }
            }
        }

        CatchInstruction FindCatchInstruction(CFGNode node)
        {
            foreach (var instruction in node.Instructions)
            {
                if (instruction is CatchInstruction) return (instruction as CatchInstruction);
            }
            return null;
        }

       
        // Creating the heap graph for recursive struct types.
        // Assumption: fld is static and fld type is struct
        void CreateStruct(ITypeDefinition containingType, IFieldDefinition fld)
        {
            ITypeDefinition fldType = fld.Type.ResolvedType;
            TypeRefWrapper ctyW = WrapperProvider.getTypeRefW(containingType);
            FieldRefWrapper fldW = WrapperProvider.getFieldRefW(fld);
            TypeRefWrapper fldTypeRefW = WrapperProvider.getTypeRefW(fldType);
            HeapElemWrapper hpElemW = WrapperProvider.getHeapElemW(fld);
            ProgramDoms.domH.Add(hpElemW);
            ProgramRels.relStructH.Add(hpElemW);
            ProgramRels.relHT.Add(hpElemW, fldTypeRefW);
            ProgramRels.relTStructFH.Add(ctyW, fldW, hpElemW);
            
            foreach (IFieldDefinition subFld in fldType.Fields)
            {
                ITypeDefinition subFldType = subFld.Type.ResolvedType;
                if (!Stubber.SuppressF(subFldType) && subFldType.IsStruct && !subFld.IsStatic) CreateStructInt(hpElemW, subFld, 1);
            }
        }

        // Assumption: fld is instance field and is of type struct
        void CreateStruct(HeapElemWrapper containingHpW, IFieldDefinition fld)
        {
            CreateStructInt(containingHpW, fld, 0);
        }

        // Assumption: fld is instance field and is of type struct
        void CreateStructInt(HeapElemWrapper containingHpW, IFieldDefinition fld, int nestingDepth)
        {
            ITypeDefinition fldType = fld.Type.ResolvedType;
            TypeRefWrapper fldTypeRefW = WrapperProvider.getTypeRefW(fldType);
            HeapElemWrapper hpFldW = WrapperProvider.getHeapElemW(fld);
            FieldRefWrapper fldW = WrapperProvider.getFieldRefW(fld);
            ProgramDoms.domH.Add(hpFldW);
            ProgramRels.relStructH.Add(hpFldW);
            ProgramRels.relHT.Add(hpFldW, fldTypeRefW);
            ProgramRels.relStructHFH.Add(containingHpW, fldW, hpFldW);
            nestingDepth++;
            foreach (IFieldDefinition subFld in fldType.Fields)
            {
                ITypeDefinition subFldType = subFld.Type.ResolvedType;
                if (!Stubber.SuppressF(subFldType) && subFldType.IsStruct && !subFld.IsStatic
                    && fldType != subFldType && nestingDepth < 4)
                    CreateStructInt(hpFldW, subFld, nestingDepth);
            }
        }

        // Assumption: lclVar is a local variable of struct type in method (wrapper) methW
        void CreateStruct(MethodRefWrapper methW, IVariable lclVar)
        {
            ITypeDefinition varTypeDef = lclVar.Type.ResolvedType;
            TypeRefWrapper varTypeRefW = WrapperProvider.getTypeRefW(varTypeDef);
            VariableWrapper lclW = WrapperProvider.getVarW(lclVar);
            HeapElemWrapper hpW = WrapperProvider.getHeapElemW(lclVar);
            ProgramDoms.domH.Add(hpW);
            ProgramRels.relMAlloc.Add(methW, lclW, hpW);
            ProgramRels.relHT.Add(hpW, varTypeRefW);
            ProgramRels.relStructH.Add(hpW);
            ProgramRels.relStructV.Add(lclW);
            foreach (IFieldDefinition subFld in varTypeDef.Fields)
            {
                ITypeDefinition subFldType = subFld.Type.ResolvedType;
                if (!Stubber.SuppressF(subFldType) && subFldType.IsStruct && !subFld.IsStatic) CreateStructInt(hpW, subFld, 1);
            }
        }

        // Assumption: containingHpW is an array whose elements are of struct type (elemType)
        // Note: Whether it is a static/instance field or a local variable, the allocation for an array happens in a method.
        void CreateStruct(MethodRefWrapper methW, HeapElemWrapper containingHpW, ITypeDefinition elemType, HeapElemWrapper elemW)
        {
            FieldRefWrapper fldW = ProgramDoms.domF.GetVal(0);
            ProgramDoms.domH.Add(elemW);
            ProgramRels.relStructH.Add(elemW);
            ProgramRels.relMStructHFH.Add(methW, containingHpW, fldW, elemW);
            TypeRefWrapper elemTypeW = WrapperProvider.getTypeRefW(elemType);
            ProgramRels.relHT.Add(elemW, elemTypeW);
            foreach (IFieldDefinition subFld in elemType.Fields)
            {
                ITypeDefinition subFldType = subFld.Type.ResolvedType;
                if (!Stubber.SuppressF(subFldType) && subFldType.IsStruct && !subFld.IsStatic) CreateStructInt(elemW, subFld, 1);
            }
        }
    }
}
