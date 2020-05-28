// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelThisIVH : Rel
    {
        public RelThisIVH() : base(3, "thisIVH")
        {
            domNames = new string[3];
            domNames[0] = ProgramDoms.domI.GetName();
            domNames[1] = ProgramDoms.domV.GetName();
            domNames[2] = ProgramDoms.domH.GetName();
        }

        public bool Add(InstructionWrapper invkW, VariableWrapper argW, HeapElemWrapper hpW)
        {
            int[] iarr = new int[3];

            iarr[0] = ProgramDoms.domI.IndexOf(invkW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domV.IndexOf(argW);
            if (iarr[1] == -1) return false;
            iarr[2] = ProgramDoms.domH.IndexOf(hpW);
            if (iarr[2] == -1) return false;
            return base.Add(iarr);
        }
    }
}
