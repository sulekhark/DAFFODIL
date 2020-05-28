// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelFinalEH : Rel
    {
        public RelFinalEH() : base(3, "FinalEH")
        {
            domNames = new string[3];
            domNames[0] = ProgramDoms.domEH.GetName();
            domNames[1] = ProgramDoms.domT.GetName();
            domNames[2] = ProgramDoms.domP.GetName();
        }

        public bool Add(ExHandlerWrapper ehW, TypeRefWrapper typeRefW, InstructionWrapper instW)
        {
            int[] iarr = new int[3];

            iarr[0] = ProgramDoms.domEH.IndexOf(ehW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domT.IndexOf(typeRefW);
            if (iarr[1] == -1) return false;
            iarr[2] = ProgramDoms.domP.IndexOf(instW);
            if (iarr[2] == -1) return false;
            return base.Add(iarr);
        }
    }
}
