// Author: Sulekha Kulkarni
// Date: Nov 2019


using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.ThreeAddressCode.Instructions;
using Daffodil.DatalogAnalysisFW.AnalysisNetConsole;
using Daffodil.DatalogAnalysisFW.ProgramFacts;

using Microsoft.Cci;

namespace Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers
{
    public class InstructionWrapper : IWrapper
    {
        readonly string instStr;
        readonly int srcLine;
        readonly string srcFile;
        readonly string className;
        readonly string methodName;

        public InstructionWrapper(Instruction inst, IMethodDefinition meth)
        {
            instStr = inst.ToString();
            methodName = meth.FullName();
            className = meth.ContainingTypeDefinition.FullName();
            if (inst.Location != null)
            {
                srcLine = inst.Location.StartLine;
                srcFile = inst.Location.PrimarySourceDocument.Location;
            }
            else
            {
                srcLine = -1;
                srcFile = "NA";
            }
        }

        public override string ToString()
        {
            return instStr;
        }

        public string GetDesc()
        {
            string s = "CLASS:" + className + " METHOD:" + methodName + " SRCFILE:" + srcFile + " SRCLINE:" + srcLine;
            return s;
        }
    }
}
