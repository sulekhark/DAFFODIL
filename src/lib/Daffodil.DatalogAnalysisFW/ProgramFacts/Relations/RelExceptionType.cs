// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelExceptionType : Rel
    {
        public RelExceptionType() : base(1, "ExceptionType")
        {
            domNames = new string[1];
            domNames[0] = ProgramDoms.domT.GetName();
        }

        public bool Add(TypeRefWrapper typeRefW)
        {
            int[] iarr = new int[1];

            iarr[0] = ProgramDoms.domT.IndexOf(typeRefW);
            if (iarr[0] == -1) return false;
            return base.Add(iarr);
        }
    }
}
