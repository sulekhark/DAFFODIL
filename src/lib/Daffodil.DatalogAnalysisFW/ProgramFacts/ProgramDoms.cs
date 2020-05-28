// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using Daffodil.DatalogAnalysisFW.ProgramFacts.Doms;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts
{
    public static class ProgramDoms
    {
        public static DomM domM;   // domain of methods
        public static DomP domP;   // domain of program points
        public static DomI domI;   // domain of invoke statements
        public static DomH domH;   // domain of allocation sites
        public static DomT domT;   // domain of types
        public static DomV domV;   // domain of local variables
        public static DomF domF;   // domain of fields
        public static DomZ domZ;   // domain of integer indexes
        public static DomX domX;   // domain of addresses
        public static DomEH domEH; // domain of exception handlers

        public static void Initialize()
        {
            domM = new DomM();
            domP = new DomP();
            domI = new DomI();
            domH = new DomH();
            domT = new DomT();
            domV = new DomV();
            domF = new DomF();
            domZ = new DomZ();
            domX = new DomX();
            domEH = new DomEH();
        }

        public static void Save()
        {
            domM.Save();
            domP.Save();
            domI.Save();
            domH.Save();
            domT.Save();
            domV.Save();
            domF.Save();
            domZ.Save();
            domX.Save();
            domEH.Save();
        }

        public static string DomUniqueString(string domName, int idx)
        {
            if (domName == ProgramDoms.domM.GetName())
            {
                return ProgramDoms.domM.ToUniqueString(idx);
            }
            else if (domName == ProgramDoms.domP.GetName())
            {
                return ProgramDoms.domP.ToUniqueString(idx);
            }
            else if (domName == ProgramDoms.domI.GetName())
            {
                return ProgramDoms.domI.ToUniqueString(idx);
            }
            else if (domName == ProgramDoms.domH.GetName())
            {
                return ProgramDoms.domH.ToUniqueString(idx);
            }
            else if (domName == ProgramDoms.domT.GetName())
            {
                return ProgramDoms.domT.ToUniqueString(idx);
            }
            else if (domName == ProgramDoms.domV.GetName())
            {
                return ProgramDoms.domV.ToUniqueString(idx);
            }
            else if (domName == ProgramDoms.domF.GetName())
            {
                return ProgramDoms.domF.ToUniqueString(idx);
            }
            else if (domName == ProgramDoms.domZ.GetName())
            {
                return ProgramDoms.domZ.ToUniqueString(idx);
            }
            else if (domName == ProgramDoms.domX.GetName())
            {
                return ProgramDoms.domX.ToUniqueString(idx);
            }
            else if (domName == ProgramDoms.domEH.GetName())
            {
                return ProgramDoms.domEH.ToUniqueString(idx);
            }
            else
            {
                return "UNKNOWN";
            }
        }

    }
}
