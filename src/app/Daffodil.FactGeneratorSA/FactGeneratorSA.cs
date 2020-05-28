// Author: Sulekha Kulkarni
// Date: Nov 2019

﻿using Daffodil.DatalogAnalysisFW.Common;
using Daffodil.DatalogAnalysisFW.ProgramFacts;
using Daffodil.DatalogAnalysisFW.AnalysisNetConsole;
using System.IO;

namespace Daffodil.FactGeneratorSA
{
    class ExFlowAnalysisSA
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ConfigParams.LoadConfig(@"C:\Users\sulek\work\DAFFODIL\src\test\T4\daffodil.cfg");
            }
            else if (args.Length == 1)
            {
                ConfigParams.LoadConfig(args[0]);
            }
            else
            {
                System.Console.WriteLine("Wrong number of input args. Exiting.");
                System.Environment.Exit(1);
            }
            ProgramDoms.Initialize();
            ProgramRels.Initialize();
            ByteCodeAnalyzer.GenerateEDBFacts(ConfigParams.AssemblyPath);
            ProgramDoms.Save();
            ProgramRels.Save();
        }
    }
}
