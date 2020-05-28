// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Relations
{
    public class RelAddrOfMX : Rel
    {
        public RelAddrOfMX() : base(2, "AddrOfMX")
        {
            domNames = new string[2];
            domNames[0] = ProgramDoms.domM.GetName();
            domNames[1] = ProgramDoms.domX.GetName();
        }

        public bool Add(MethodRefWrapper mRefW, AddressWrapper addrW)
        {
            int[] iarr = new int[2];

            iarr[0] = ProgramDoms.domM.IndexOf(mRefW);
            if (iarr[0] == -1) return false;
            iarr[1] = ProgramDoms.domX.IndexOf(addrW);
            if (iarr[1] == -1) return false;
            return base.Add(iarr);
        }
    }
}
