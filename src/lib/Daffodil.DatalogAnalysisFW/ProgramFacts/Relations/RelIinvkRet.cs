// Author: Sulekha Kulkarni
// Date: Nov 2019



using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelIinvkRet : Rel
    {
        public RelIinvkRet() : base(3, "IinvkRet")
        {
            domNames = new string[3];
            domNames[0] = ProgramDoms.domI.GetName();
            domNames[1] = ProgramDoms.domZ.GetName();
            domNames[2] = ProgramDoms.domV.GetName();
        }

        public bool Add(InstructionWrapper invkW, int argNum, VariableWrapper argW)
        {
            int[] iarr = new int[3];

            iarr[0] = ProgramDoms.domI.IndexOf(invkW);
            if (iarr[0] == -1) return false;
            iarr[1] = argNum;
            iarr[2] = ProgramDoms.domV.IndexOf(argW);
            if (iarr[2] == -1) return false;
            return base.Add(iarr);
        }
    }
}

