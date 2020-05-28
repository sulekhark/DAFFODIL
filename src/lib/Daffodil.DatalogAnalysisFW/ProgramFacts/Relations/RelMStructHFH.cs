// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelMStructHFH : Rel
    {
        public RelMStructHFH() : base(4, "MStructHFH")
        {
            domNames = new string[4];
            domNames[0] = ProgramDoms.domM.GetName();
            domNames[1] = ProgramDoms.domH.GetName();
            domNames[2] = ProgramDoms.domF.GetName();
            domNames[3] = ProgramDoms.domH.GetName();
        }

        public bool Add(MethodRefWrapper methW, HeapElemWrapper arrW1, FieldRefWrapper fldRefW, HeapElemWrapper elemW2)
        {
            int[] iarr = new int[4];

            iarr[0] = ProgramDoms.domM.IndexOf(methW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domH.IndexOf(arrW1);
            if (iarr[1] == -1) return false;
            iarr[2] = ProgramDoms.domF.IndexOf(fldRefW);
            if (iarr[2] == -1) return false;
            iarr[3] = ProgramDoms.domH.IndexOf(elemW2);
            if (iarr[3] == -1) return false;
            return base.Add(iarr);
        }
    }
}
