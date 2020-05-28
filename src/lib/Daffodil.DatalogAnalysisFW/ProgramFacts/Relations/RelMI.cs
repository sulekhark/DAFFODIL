// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;


namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelMI : Rel
    {
        public RelMI() : base(2, "MI")
        {
            domNames = new string[2];
            domNames[0] = ProgramDoms.domM.GetName();
            domNames[1] = ProgramDoms.domI.GetName();
        }

        public bool Add(MethodRefWrapper mRefW, InstructionWrapper invkW)
        {
            int[] iarr = new int[2];

            iarr[0] = ProgramDoms.domM.IndexOf(mRefW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domI.IndexOf(invkW);
            if (iarr[1] == -1) return false;
            return base.Add(iarr);
        }
    }
}
