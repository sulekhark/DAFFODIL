// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;


namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelMAddrTakenLocal : Rel
    {
        public RelMAddrTakenLocal() : base(3, "MAddrTakenLocal")
        {
            domNames = new string[3];
            domNames[0] = ProgramDoms.domM.GetName();
            domNames[1] = ProgramDoms.domV.GetName();
            domNames[2] = ProgramDoms.domV.GetName();
        }

        public bool Add(MethodRefWrapper mRefW, VariableWrapper lhsW, VariableWrapper rhsW)
        {
            int[] iarr = new int[3];

            iarr[0] = ProgramDoms.domM.IndexOf(mRefW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domV.IndexOf(lhsW);
            if (iarr[1] == -1) return false;
            iarr[2] = ProgramDoms.domV.IndexOf(rhsW);
            if (iarr[2] == -1) return false;
            return base.Add(iarr);
        }
    }
}
