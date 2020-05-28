// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;


namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelStaticTF : Rel
    {
        public RelStaticTF() : base(2, "staticTF")
        {
            domNames = new string[2];
            domNames[0] = ProgramDoms.domT.GetName();
            domNames[1] = ProgramDoms.domF.GetName();
        }

        public bool Add(TypeRefWrapper typRefW, FieldRefWrapper fldRefW)
        {
            int[] iarr = new int[2];

            iarr[0] = ProgramDoms.domT.IndexOf(typRefW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domF.IndexOf(fldRefW);
            if (iarr[1] == -1) return false;
            return base.Add(iarr);
        }
    }
}
