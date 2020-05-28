// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelThisRefV : Rel
    {
        public RelThisRefV() : base(1, "thisRefV")
        {
            domNames = new string[1];
            domNames[0] = ProgramDoms.domV.GetName();
        }

        public bool Add(VariableWrapper varW)
        {
            int[] iarr = new int[1];

            iarr[0] = ProgramDoms.domV.IndexOf(varW);
            if (iarr[0] == -1) return false;
            return base.Add(iarr);
        }
    }
}
