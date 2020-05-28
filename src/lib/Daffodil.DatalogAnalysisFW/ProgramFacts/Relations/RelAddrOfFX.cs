// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelAddrOfFX : Rel
    {
        public RelAddrOfFX() : base(2, "AddrOfFX")
        {
            domNames = new string[2];
            domNames[0] = ProgramDoms.domF.GetName();
            domNames[1] = ProgramDoms.domX.GetName();
        }

        public bool Add(FieldRefWrapper fldRefW, AddressWrapper addrW)
        {
            int[] iarr = new int[2];

            iarr[0] = ProgramDoms.domF.IndexOf(fldRefW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domX.IndexOf(addrW);
            if (iarr[1] == -1) return false;
            return base.Add(iarr);
        }
    }
}
