// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelTStructFH : Rel
    {
        public RelTStructFH() : base(3, "TStructFH")
        {
            domNames = new string[3];
            domNames[0] = ProgramDoms.domT.GetName();
            domNames[1] = ProgramDoms.domF.GetName();
            domNames[2] = ProgramDoms.domH.GetName();
        }

        public bool Add(TypeRefWrapper tRefW, FieldRefWrapper fldRefW, HeapElemWrapper allocW)
        {
            int[] iarr = new int[3];

            iarr[0] = ProgramDoms.domT.IndexOf(tRefW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domF.IndexOf(fldRefW);
            if (iarr[1] == -1) return false;
            iarr[2] = ProgramDoms.domH.IndexOf(allocW);
            if (iarr[2] == -1) return false;
            return base.Add(iarr);
        }
    }
}
