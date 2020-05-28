// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Cci;
using Daffodil.DatalogAnalysisFW.AnalysisNetBackend;
using Daffodil.DatalogAnalysisFW.Common;

namespace Daffodil.DatalogAnalysisFW.AnalysisNetConsole
{
    public static class ByteCodeAnalyzer
    {
        static RTAAnalyzer rtaAnalyzer;
        static FactGenerator factGen;
        public static void GenerateEDBFacts(string input)
        {
            using (var host = new PeReader.DefaultHost())
            {
                Types.Initialize(host);
                var visitor = new ClassAndMethodVisitor(host);
                IModule rootModule = GetModule(host, input);
                IModule stubsModule = GetModule(host, ConfigParams.StubsPath);
                DoRTA(host, visitor, rootModule, stubsModule);
                GenerateFacts(visitor);
            }
        }

       
        static IModule GetModule(IMetadataHost host, string assemblyPath)
        {
            IModule module = host.LoadUnitFrom(assemblyPath) as IModule;

            if (module == null || module == Dummy.Module || module == Dummy.Assembly)
                throw new System.Exception("Invalid module (perhaps, file does not exist / file path incorrect).");
            return module;
        }

        static void Initialize(ISet<ITypeDefinition> classesSet, ISet<ITypeDefinition> entryPtList, IModule rootModule, bool rootIsExe)
        {
            foreach (ITypeDefinition ty in rootModule.GetAllTypes())
            {
                if (ty is INamedTypeDefinition && !ty.IsGeneric && !ty.IsAbstract && ty.FullName() != "<Module>")
                {
                    if (rootIsExe)
                    {
                        if (Utils.GetMethodByName(ty, "Main") != null)
                        {
                            classesSet.Add(ty);
                            entryPtList.Add(ty);
                        }
                    }
                    else
                    {
                        classesSet.Add(ty);
                        entryPtList.Add(ty);
                    }
                }
            }
        }

        static void DoRTA(IMetadataHost host, ClassAndMethodVisitor visitor, IModule rootModule, IModule stubsModule)
        {
            bool rootIsExe = false;
            if (rootModule.Kind == ModuleKind.ConsoleApplication) rootIsExe = true;
            StreamWriter rtaLogSW = new StreamWriter(Path.Combine(ConfigParams.LogDir, "rta_log.txt"));
            rtaAnalyzer = new RTAAnalyzer(rootIsExe, rtaLogSW, host);
            visitor.SetupRTAAnalyzer(rtaAnalyzer);
            Stubber.SetupRTAAnalyzer(rtaAnalyzer);
            Stubs.SetupInternFactory(host.InternFactory);
            GenericMethods.SetupInternFactory(host.InternFactory);
            Stubs.SetupStubs(stubsModule);

            bool diskLoadSuccessful = false;
            if (ConfigParams.LoadSavedScope) diskLoadSuccessful = rtaAnalyzer.LoadSavedScope(host);
            if (!diskLoadSuccessful) Initialize(rtaAnalyzer.classes, rtaAnalyzer.entryPtClasses, rootModule, rootIsExe);

            int iterationCount = 0;
            bool changeInCount = true;
            int startClassCnt, startMethCnt;
            while (changeInCount)
            {
                rtaLogSW.WriteLine();
                rtaLogSW.WriteLine("Starting RTA ITERATION:{0}", iterationCount);
                startClassCnt = rtaAnalyzer.classes.Count;
                startMethCnt = rtaAnalyzer.methods.Count;
                rtaLogSW.WriteLine("Counts: classes:{0}   methods:{1}", startClassCnt, startMethCnt);
                rtaAnalyzer.classWorkList.Clear();
                rtaAnalyzer.visitedClasses.Clear();
                rtaAnalyzer.ignoredClasses.Clear();
                rtaAnalyzer.visitedMethods.Clear();
                rtaAnalyzer.ignoredMethods.Clear();
                CopyAll(rtaAnalyzer.classes, rtaAnalyzer.classWorkList);
                while (rtaAnalyzer.classWorkList.Count > 0)
                {
                    ITypeDefinition ty = rtaAnalyzer.classWorkList.First<ITypeDefinition>();
                    rtaAnalyzer.classWorkList.RemoveAt(0);
                    visitor.Traverse(ty);
                }
                if (rtaAnalyzer.classes.Count == startClassCnt && rtaAnalyzer.methods.Count == startMethCnt) changeInCount = false;
                iterationCount++;
            }
            Copy(rtaAnalyzer.allocClasses, rtaAnalyzer.classes);
            rtaLogSW.WriteLine();
            rtaLogSW.WriteLine();
            foreach (IMethodDefinition m in rtaAnalyzer.methods)
            {
                rtaLogSW.WriteLine(m.FullName());
            }
            rtaLogSW.WriteLine();
            rtaLogSW.WriteLine();
            foreach (IMethodDefinition m in rtaAnalyzer.entryPtMethods)
            {
                rtaLogSW.WriteLine(m.FullName());
            }
            rtaLogSW.WriteLine();
            rtaLogSW.WriteLine();
            foreach (ITypeDefinition cl in rtaAnalyzer.classes)
            {
                rtaLogSW.WriteLine(cl.FullName());
            }

            rtaLogSW.WriteLine();
            rtaLogSW.WriteLine("ALLOC CLASSES");
            foreach (ITypeDefinition cl in rtaAnalyzer.allocClasses)
            {
                rtaLogSW.WriteLine(cl.FullName());
            }
            rtaLogSW.WriteLine("+++++++++++++++ RTA DONE ++++++++++++++++++");

            if (!diskLoadSuccessful) rtaAnalyzer.SaveScope(host);
            rtaLogSW.Close();
        }

        static void GenerateFacts(ClassAndMethodVisitor visitor)
        {
            StreamWriter tacLogSW = new StreamWriter(Path.Combine(ConfigParams.LogDir, "tac_log.txt"));
            StreamWriter factGenLogSW = new StreamWriter(Path.Combine(ConfigParams.LogDir, "factgen_log.txt"));
            factGen = new FactGenerator(tacLogSW, factGenLogSW);

            factGen.classes = rtaAnalyzer.classes;
            factGen.methods = rtaAnalyzer.methods;
            factGen.types = rtaAnalyzer.types;
            factGen.allocClasses = rtaAnalyzer.allocClasses;
            factGen.entryPtMethods = rtaAnalyzer.entryPtMethods;
            factGen.addrTakenInstFlds = rtaAnalyzer.addrTakenInstFlds;
            factGen.addrTakenStatFlds = rtaAnalyzer.addrTakenStatFlds;
            factGen.addrTakenMethods = rtaAnalyzer.addrTakenMethods;
            factGen.addrTakenLocals = rtaAnalyzer.addrTakenLocals;

            factGen.GenerateTypeAndMethodFacts();
            factGen.GenerateChaFacts();
            visitor.SetupRTAAnalyzer(null);
            visitor.SetupFactGenerator(factGen);
            foreach (ITypeDefinition ty in rtaAnalyzer.classes)
            {
                visitor.Traverse(ty);
            }
            factGen.CheckDomX();
            tacLogSW.Close();
        }

        static void CopyAll(ISet<ITypeDefinition> srcSet, IList<ITypeDefinition> dstList)
        {
            foreach (ITypeDefinition ty in srcSet) dstList.Add(ty);
        }

        static void Copy(ISet<ITypeDefinition> srcSet, ISet<ITypeDefinition> dstSet)
        {
            foreach (ITypeDefinition ty in srcSet) if (!dstSet.Contains(ty)) dstSet.Add(ty);
        }
    }
}

