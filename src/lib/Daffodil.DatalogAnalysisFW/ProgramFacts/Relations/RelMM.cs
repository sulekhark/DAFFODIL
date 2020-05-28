// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelMM : Rel
    {
        public RelMM() : base(2, "MM")
        {
            domNames = new string[2];
            domNames[0] = ProgramDoms.domM.GetName();
            domNames[1] = ProgramDoms.domM.GetName();
        }

        public bool Add(MethodRefWrapper mCaller, MethodRefWrapper mCallee)
        {
            int[] iarr = new int[2];

            iarr[0] = ProgramDoms.domM.IndexOf(mCaller);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domM.IndexOf(mCallee);
            if (iarr[1] == -1) return false;
            return base.Add(iarr);
        }
    }
}
