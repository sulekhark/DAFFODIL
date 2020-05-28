#!/bin/bash

rm -f full_run.out

z3 ExcAnalysisIntraProc.datalog > exc_intra.out 2>&1
python3 parse_z3_out.py ExcAnalysisIntraProc.datalog exc_intra.out >> full_run.out 2>&1

z3 CIPtrAnalysis.datalog > cipa_analysis.out 2>&1
python3 parse_z3_out.py CIPtrAnalysis.datalog cipa_analysis.out >> full_run.out 2>&1

