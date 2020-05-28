// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System.Collections.Generic;
using Daffodil.DatalogAnalysisFW.ProgramFacts.Relations;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts
{
    public static class ProgramRels
    {
        public static RelTM relTM;
        public static RelMI relMI;
        public static RelVirtIM relVirtIM;
        public static RelStatIM relStatIM;
        public static RelMM relMM;
        public static RelMMove relMMove;
        public static RelMAlloc relMAlloc;
        public static RelMInstFldRead relMInstFldRead;
        public static RelMStatFldRead relMStatFldRead;
        public static RelMInstFldWrite relMInstFldWrite;
        public static RelMStatFldWrite relMStatFldWrite;
        public static RelHT relHT;
        public static RelIinvkArg0 relIinvkArg0;
        public static RelIinvkArg relIinvkArg;
        public static RelIinvkRet relIinvkRet;
        public static RelMmethArg relMmethArg;
        public static RelMmethRet relMmethRet;
        public static RelClassT relClassT;
        public static RelSub relSub;
        public static RelEntryPtM relEntryPtM;
        public static RelCha relCha;
        public static RelStaticTM relStaticTM;
        public static RelClinitTM relClinitTM;
        public static RelStaticTF relStaticTF;
        public static RelVT relVT;
        public static RelReachableT relReachableT;
        public static RelReachableI relReachableI;
        public static RelReachableM relReachableM;
        public static RelRootM relRootM;
        public static RelVH relVH;
        public static RelFH relFH;
        public static RelHFH relHFH;
        public static RelIM relIM;
	
        public static RelVX relVX;
        public static RelFX relFX;
        public static RelHFX relHFX;
        public static RelAddrOfVX relAddrOfVX;
        public static RelAddrOfFX relAddrOfFX;
        public static RelAddrOfHFX relAddrOfHFX;
        public static RelAddrOfMX relAddrOfMX;
	    public static RelMAddrTakenInstFld relMAddrTakenInstFld;
	    public static RelMAddrTakenStatFld relMAddrTakenStatFld;
	    public static RelMAddrTakenLocal relMAddrTakenLocal;
        public static RelMAddrTakenFunc relMAddrTakenFunc;
        public static RelMDerefLeft relMDerefLeft;
	    public static RelMDerefRight relMDerefRight;
	    public static RelDelegateIV relDelegateIV;
	    public static RelStructV relStructV;
        public static RelStructH relStructH;
        public static RelStrMove relStrMove;
        public static RelMove relMove;
        public static RelVHfilter relVHfilter;
        public static RelVarEH relVarEH;
        public static RelThrowPV relThrowPV;
        public static RelInRange relInRange;
        public static RelPrevEH relPrevEH;
        public static RelTypeEH relTypeEH;
        public static RelFinalEH relFinalEH;
        public static RelPI relPI;
        public static RelCallAt relCallAt;
        public static RelMEH relMEH;
        public static RelExceptionType relExceptionType;
        public static RelEscapeMTP relEscapeMTP;
        public static RelFinalMTP relFinalMTP;
        public static RelMStructHFH relMStructHFH;
        public static RelStructRefV relStructRefV;
        public static RelThisIVH relThisIVH;
        public static RelThisRefV relThisRefV;
        public static RelTStructFH relTStructFH;
        public static RelStructHFH relStructHFH;
        public static RelMV relMV;
        public static RelFT relFT;


        public static Dictionary<string, Rel> nameToRelMap;
        public static void Initialize()
        {
            nameToRelMap = new Dictionary<string, Rel>();

            relTM = new RelTM();
            nameToRelMap.Add(relTM.GetName(), relTM);

            relMI = new RelMI();
            nameToRelMap.Add(relMI.GetName(), relMI);

            relVirtIM = new RelVirtIM();
            nameToRelMap.Add(relVirtIM.GetName(), relVirtIM);

            relStatIM = new RelStatIM();
            nameToRelMap.Add(relStatIM.GetName(), relStatIM);

            relMM = new RelMM();
            nameToRelMap.Add(relMM.GetName(), relMM);

            relMMove = new RelMMove();
            nameToRelMap.Add(relMMove.GetName(), relMMove);

            relMAlloc = new RelMAlloc();
            nameToRelMap.Add(relMAlloc.GetName(), relMAlloc);

            relMInstFldRead = new RelMInstFldRead();
            nameToRelMap.Add(relMInstFldRead.GetName(), relMInstFldRead);

            relMStatFldRead = new RelMStatFldRead();
            nameToRelMap.Add(relMStatFldRead.GetName(), relMStatFldRead);

            relMInstFldWrite = new RelMInstFldWrite();
            nameToRelMap.Add(relMInstFldWrite.GetName(), relMInstFldWrite);

            relMStatFldWrite = new RelMStatFldWrite();
            nameToRelMap.Add(relMStatFldWrite.GetName(), relMStatFldWrite);

            relHT = new RelHT();
            nameToRelMap.Add(relHT.GetName(), relHT);

            relIinvkArg0 = new RelIinvkArg0();
            nameToRelMap.Add(relIinvkArg0.GetName(), relIinvkArg0);

            relIinvkArg = new RelIinvkArg();
            nameToRelMap.Add(relIinvkArg.GetName(), relIinvkArg);

            relIinvkRet = new RelIinvkRet();
            nameToRelMap.Add(relIinvkRet.GetName(), relIinvkRet);

            relMmethArg = new RelMmethArg();
            nameToRelMap.Add(relMmethArg.GetName(), relMmethArg);

            relMmethRet = new RelMmethRet();
            nameToRelMap.Add(relMmethRet.GetName(), relMmethRet);

            relClassT = new RelClassT();
            nameToRelMap.Add(relClassT.GetName(), relClassT);

            relSub = new RelSub();
            nameToRelMap.Add(relSub.GetName(), relSub);

            relEntryPtM = new RelEntryPtM();
            nameToRelMap.Add(relEntryPtM.GetName(), relEntryPtM);

            relCha = new RelCha();
            nameToRelMap.Add(relCha.GetName(), relCha);

            relStaticTM = new RelStaticTM();
            nameToRelMap.Add(relStaticTM.GetName(), relStaticTM);

            relStaticTF = new RelStaticTF();
            nameToRelMap.Add(relStaticTF.GetName(), relStaticTF);

            relClinitTM = new RelClinitTM();
            nameToRelMap.Add(relClinitTM.GetName(), relClinitTM);

            relVT = new RelVT();
            nameToRelMap.Add(relVT.GetName(), relVT);

            relReachableT = new RelReachableT();
            nameToRelMap.Add(relReachableT.GetName(), relReachableT);

            relReachableI = new RelReachableI();
            nameToRelMap.Add(relReachableI.GetName(), relReachableI);

            relReachableM = new RelReachableM();
            nameToRelMap.Add(relReachableM.GetName(), relReachableM);

            relRootM = new RelRootM();
            nameToRelMap.Add(relRootM.GetName(), relRootM);

            relVH = new RelVH();
            nameToRelMap.Add(relVH.GetName(), relVH);

            relFH = new RelFH();
            nameToRelMap.Add(relFH.GetName(), relFH);

            relHFH = new RelHFH();
            nameToRelMap.Add(relHFH.GetName(), relHFH);

            relIM = new RelIM();
            nameToRelMap.Add(relIM.GetName(), relIM);
          
            relVX = new RelVX();
            nameToRelMap.Add(relVX.GetName(), relVX);

            relFX = new RelFX();
            nameToRelMap.Add(relFX.GetName(), relFX);

            relHFX = new RelHFX();
            nameToRelMap.Add(relHFX.GetName(), relHFX);

            relAddrOfVX = new RelAddrOfVX();
            nameToRelMap.Add(relAddrOfVX.GetName(), relAddrOfVX);

            relAddrOfFX = new RelAddrOfFX();
            nameToRelMap.Add(relAddrOfFX.GetName(), relAddrOfFX);

            relAddrOfHFX = new RelAddrOfHFX();
            nameToRelMap.Add(relAddrOfHFX.GetName(), relAddrOfHFX);

            relAddrOfMX = new RelAddrOfMX();
            nameToRelMap.Add(relAddrOfMX.GetName(), relAddrOfMX);

            relMAddrTakenLocal = new RelMAddrTakenLocal();
            nameToRelMap.Add(relMAddrTakenLocal.GetName(), relMAddrTakenLocal);

            relMAddrTakenFunc = new RelMAddrTakenFunc();
            nameToRelMap.Add(relMAddrTakenFunc.GetName(), relMAddrTakenFunc);

            relMAddrTakenInstFld = new RelMAddrTakenInstFld();
            nameToRelMap.Add(relMAddrTakenInstFld.GetName(), relMAddrTakenInstFld);

            relMAddrTakenStatFld = new RelMAddrTakenStatFld();
            nameToRelMap.Add(relMAddrTakenStatFld.GetName(), relMAddrTakenStatFld);

            relMDerefRight = new RelMDerefRight();
            nameToRelMap.Add(relMDerefRight.GetName(), relMDerefRight);

            relMDerefLeft = new RelMDerefLeft();
            nameToRelMap.Add(relMDerefLeft.GetName(), relMDerefLeft);

            relDelegateIV = new RelDelegateIV();
            nameToRelMap.Add(relDelegateIV.GetName(), relDelegateIV);

            relStructV = new RelStructV();
            nameToRelMap.Add(relStructV.GetName(), relStructV);

            relStructH = new RelStructH();
            nameToRelMap.Add(relStructH.GetName(), relStructH);

            relStrMove = new RelStrMove();
            nameToRelMap.Add(relStrMove.GetName(), relStrMove);

            relMove = new RelMove();
            nameToRelMap.Add(relMove.GetName(), relMove);

            relVHfilter = new RelVHfilter();
            nameToRelMap.Add(relVHfilter.GetName(), relVHfilter);

            relVarEH = new RelVarEH();
            nameToRelMap.Add(relVarEH.GetName(), relVarEH);

            relThrowPV = new RelThrowPV();
            nameToRelMap.Add(relThrowPV.GetName(), relThrowPV);

            relInRange = new RelInRange();
            nameToRelMap.Add(relInRange.GetName(), relInRange);

            relPrevEH = new RelPrevEH();
            nameToRelMap.Add(relPrevEH.GetName(), relPrevEH);

            relTypeEH = new RelTypeEH();
            nameToRelMap.Add(relTypeEH.GetName(), relTypeEH);

            relFinalEH = new RelFinalEH();
            nameToRelMap.Add(relFinalEH.GetName(), relFinalEH);

            relPI = new RelPI();
            nameToRelMap.Add(relPI.GetName(), relPI);

            relCallAt = new RelCallAt();
            nameToRelMap.Add(relCallAt.GetName(), relCallAt);

            relMEH = new RelMEH();
            nameToRelMap.Add(relMEH.GetName(), relMEH);

            relExceptionType = new RelExceptionType();
            nameToRelMap.Add(relExceptionType.GetName(), relExceptionType);

            relEscapeMTP = new RelEscapeMTP();
            nameToRelMap.Add(relEscapeMTP.GetName(), relEscapeMTP);

            relFinalMTP = new RelFinalMTP();
            nameToRelMap.Add(relFinalMTP.GetName(), relFinalMTP);

            relMStructHFH = new RelMStructHFH();
            nameToRelMap.Add(relMStructHFH.GetName(), relMStructHFH);

            relStructRefV = new RelStructRefV();
            nameToRelMap.Add(relStructRefV.GetName(), relStructRefV);

            relThisIVH = new RelThisIVH();
            nameToRelMap.Add(relThisIVH.GetName(), relThisIVH);

            relThisRefV = new RelThisRefV();
            nameToRelMap.Add(relThisRefV.GetName(), relThisRefV);

            relTStructFH = new RelTStructFH();
            nameToRelMap.Add(relTStructFH.GetName(), relTStructFH);

            relStructHFH = new RelStructHFH();
            nameToRelMap.Add(relStructHFH.GetName(), relStructHFH);

            relMV = new RelMV();
            nameToRelMap.Add(relMV.GetName(), relMV);

            relFT = new RelFT();
            nameToRelMap.Add(relFT.GetName(), relFT);
        }

        public static void Save()
        {
            relTM.Save();
            relMI.Save();
            relVirtIM.Save();
            relStatIM.Save();
            relMM.Save();
            relMMove.Save();
            relMAlloc.Save();
            relMInstFldRead.Save();
            relMInstFldWrite.Save();
            relMStatFldRead.Save();
            relMStatFldWrite.Save();
            relHT.Save();
            relIinvkArg0.Save();
            relIinvkArg.Save();
            relIinvkRet.Save();
            relMmethArg.Save();
            relMmethRet.Save();
            relClassT.Save();
            relSub.Save();
            relEntryPtM.Save();
            relCha.Save();
            relStaticTM.Save();
            relStaticTF.Save();
            relClinitTM.Save();
            relVT.Save();

	        relAddrOfVX.Save();
	        relAddrOfFX.Save();
	        relAddrOfHFX.Save();
	        relAddrOfMX.Save();
	        relMAddrTakenInstFld.Save();
            relMAddrTakenLocal.Save();
            relMAddrTakenFunc.Save();
            relMAddrTakenStatFld.Save();
	        relMDerefLeft.Save();
	        relMDerefRight.Save();
	        relDelegateIV.Save();
	        relStructV.Save();
            relStructH.Save();
            relVarEH.Save();
            relThrowPV.Save();
            relInRange.Save();
            relPrevEH.Save();
            relTypeEH.Save();
            relPI.Save();
            relMEH.Save();
            relExceptionType.Save();
            relMStructHFH.Save();
            relStructRefV.Save();
            relThisRefV.Save();
            relTStructFH.Save();
            relStructHFH.Save();
            relMV.Save();
            relFT.Save();
        }
    }
}
