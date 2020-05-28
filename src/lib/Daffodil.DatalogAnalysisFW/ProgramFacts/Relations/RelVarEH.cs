// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelVarEH : Rel
    {
        public RelVarEH() : base(2, "VarEH")
        {
            domNames = new string[2];
            domNames[0] = ProgramDoms.domEH.GetName();
            domNames[1] = ProgramDoms.domV.GetName();
        }

        public bool Add(ExHandlerWrapper ehW, VariableWrapper varW)
        {
            int[] iarr = new int[2];

            iarr[0] = ProgramDoms.domEH.IndexOf(ehW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domV.IndexOf(varW);
            if (iarr[1] == -1) return false;
            return base.Add(iarr);
        }
    }
}
