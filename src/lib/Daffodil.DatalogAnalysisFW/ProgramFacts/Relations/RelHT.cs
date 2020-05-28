// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;


namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelHT : Rel
    {
        public RelHT() : base(2, "HT")
        {
            domNames = new string[2];
            domNames[0] = ProgramDoms.domH.GetName();
            domNames[1] = ProgramDoms.domT.GetName();
        }

        public bool Add(HeapElemWrapper allocW, TypeRefWrapper typRefW)
        {
            int[] iarr = new int[2];

            iarr[0] = ProgramDoms.domH.IndexOf(allocW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domT.IndexOf(typRefW);
            if (iarr[1] == -1) return false;
            
            return base.Add(iarr);
        }
    }
}
