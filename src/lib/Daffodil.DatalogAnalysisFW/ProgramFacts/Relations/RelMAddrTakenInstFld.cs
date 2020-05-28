// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;


namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelMAddrTakenInstFld : Rel
    {
        public RelMAddrTakenInstFld() : base(4, "MAddrTakenInstFld")
        {
            domNames = new string[4];
            domNames[0] = ProgramDoms.domM.GetName();
            domNames[1] = ProgramDoms.domV.GetName();
            domNames[2] = ProgramDoms.domV.GetName();
            domNames[3] = ProgramDoms.domF.GetName();
        }

        public bool Add(MethodRefWrapper mRefW, VariableWrapper lhsW, VariableWrapper rhsW, FieldRefWrapper fldW)
        {
            int[] iarr = new int[4];

            iarr[0] = ProgramDoms.domM.IndexOf(mRefW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domV.IndexOf(lhsW);
            if (iarr[1] == -1) return false;
            iarr[2] = ProgramDoms.domV.IndexOf(rhsW);
            if (iarr[2] == -1) return false;
            iarr[3] = ProgramDoms.domF.IndexOf(fldW);
            if (iarr[3] == -1) return false;
            return base.Add(iarr);
        }
    }
}
