// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelCallAt : Rel
    {
        public RelCallAt() : base(3, "CallAt")
        {
            domNames = new string[3];
            domNames[0] = ProgramDoms.domM.GetName();
            domNames[1] = ProgramDoms.domP.GetName();
            domNames[2] = ProgramDoms.domM.GetName();
        }

        public bool Add(MethodRefWrapper methW, InstructionWrapper instW, MethodRefWrapper calleeW)
        {
            int[] iarr = new int[3];

            iarr[0] = ProgramDoms.domM.IndexOf(methW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domP.IndexOf(instW);
            if (iarr[1] == -1) return false;
            iarr[2] = ProgramDoms.domM.IndexOf(calleeW);
            if (iarr[2] == -1) return false;
            return base.Add(iarr);
        }
    }
}
