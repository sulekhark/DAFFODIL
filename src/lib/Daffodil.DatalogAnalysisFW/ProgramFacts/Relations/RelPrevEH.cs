// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelPrevEH : Rel
    {
        public RelPrevEH() : base(2, "PrevEH")
        {
            domNames = new string[2];
            domNames[0] = ProgramDoms.domEH.GetName();
            domNames[1] = ProgramDoms.domEH.GetName();
        }

        public bool Add(ExHandlerWrapper ehW1, ExHandlerWrapper ehW2)
        {
            int[] iarr = new int[2];

            iarr[0] = ProgramDoms.domEH.IndexOf(ehW1);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domEH.IndexOf(ehW2);
            if (iarr[1] == -1) return false;
            return base.Add(iarr);
        }
    }
}
