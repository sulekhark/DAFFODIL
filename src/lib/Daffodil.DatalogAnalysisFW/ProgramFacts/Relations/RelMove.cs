// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;


namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelMove : Rel
    {
        public RelMove() : base(2, "Move")
        {
            domNames = new string[2];
            domNames[0] = ProgramDoms.domV.GetName();
            domNames[1] = ProgramDoms.domV.GetName();
        }

        public bool Add(VariableWrapper lhsW, VariableWrapper rhsW)
        {
            int[] iarr = new int[2];

            iarr[0] = ProgramDoms.domV.IndexOf(lhsW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domV.IndexOf(rhsW);
            if (iarr[1] == -1) return false;
            return base.Add(iarr);
        }
    }
}

