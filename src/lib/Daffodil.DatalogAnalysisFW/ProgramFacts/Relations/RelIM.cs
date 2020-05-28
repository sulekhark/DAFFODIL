// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelIM : Rel
    {
        public RelIM() : base(2, "IM")
        {
            domNames = new string[2];
            domNames[0] = ProgramDoms.domI.GetName();
            domNames[1] = ProgramDoms.domM.GetName();
        }

        public bool Add(InstructionWrapper invkW, MethodRefWrapper mRefW)
        {
            int[] iarr = new int[2];

            iarr[0] = ProgramDoms.domI.IndexOf(invkW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domM.IndexOf(mRefW);
            if (iarr[1] == -1) return false;
            return base.Add(iarr);
        }
    }
}
