// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts.Doms
{
    public class DomZ : Dom<IntWrapper>
    {
        public DomZ() : base("Z")
        {
            for (int i = 0; i < 32; i++)
            {
                base.Add(new IntWrapper(i));
            }
        }
    }
}
