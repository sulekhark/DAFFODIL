// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;


namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelMInstFldWrite : Rel
    {
        public RelMInstFldWrite() : base(4, "MInstFldWrite")
        {
            domNames = new string[4];
            domNames[0] = ProgramDoms.domM.GetName();
            domNames[1] = ProgramDoms.domV.GetName();
            domNames[2] = ProgramDoms.domF.GetName();
            domNames[3] = ProgramDoms.domV.GetName();
        }

        public bool Add(MethodRefWrapper mRefW, VariableWrapper lhsW, FieldRefWrapper fldW, VariableWrapper rhsW)
        {
            int[] iarr = new int[4];

            iarr[0] = ProgramDoms.domM.IndexOf(mRefW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domV.IndexOf(lhsW);
            if (iarr[1] == -1) return false;
            iarr[2] = ProgramDoms.domF.IndexOf(fldW);
            if (iarr[2] == -1) return false;
            iarr[3] = ProgramDoms.domV.IndexOf(rhsW);
            if (iarr[3] == -1) return false;
            return base.Add(iarr);
        }
    }
}
