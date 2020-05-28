// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelStrMove : Rel
    {
        public RelStrMove() : base(2, "StrMove")
        {
            domNames = new string[2];
            domNames[0] = ProgramDoms.domH.GetName();
            domNames[1] = ProgramDoms.domH.GetName();
        }

        public bool Add(HeapElemWrapper allocW1, FieldRefWrapper fldRefW, HeapElemWrapper allocW2)
        {
            int[] iarr = new int[2];

            iarr[0] = ProgramDoms.domH.IndexOf(allocW1);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domH.IndexOf(allocW2);
            if (iarr[1] == -1) return false;
            return base.Add(iarr);
        }
    }
}
