// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelFT : Rel
    {
        public RelFT() : base(2, "FT")
        {
            domNames = new string[2];
            domNames[0] = ProgramDoms.domF.GetName();
            domNames[1] = ProgramDoms.domT.GetName();
        }

        public bool Add(FieldRefWrapper fldRefW, TypeRefWrapper typRefW)
        {
            int[] iarr = new int[2];

            iarr[0] = ProgramDoms.domF.IndexOf(fldRefW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domT.IndexOf(typRefW);
            if (iarr[1] == -1) return false;

            return base.Add(iarr);
        }
    }
}
