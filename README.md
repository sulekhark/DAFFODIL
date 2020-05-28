# DAFFODIL (Datalog Analysis Framework for .NET IL) # 

DAFFODIL is a Datalog analysis framework written in C#. It takes as input .NET exes/dlls, does not need source code and transitively processes all dependent dlls and system libraries. It does not handle reflection. Compilation target is .NET framework version 4.5 onwards. DAFFODIL has capability for the following:
   - Computing the scope of classes and methods for analysis: Computes scope using RTA (Rapid Type Analysis).
       + Handles generic types and methods.
   - Fact generation: Converts program text into Datalog domains, and relations over those domains.
   - Pointer analysis: Contains a context-insensitive flow-insensitive Andersen-style pointer analysis together with exception analysis written in Datalog (ref: [Paper](https://people.cs.umass.edu/~yannis/doop-issta09prelim.pdf)).
       + Handles address-taken and pointer dereferencing.
       + Handles value types like structs.
       + Models delegates.


**Acknowledgements**
---

DAFFODIL uses the code from AnalysisNet to read .NET bytecode, convert it into three-address-instructions in SSA form and perform intraprocedural optimizations. The version used here has been checked out from the fork at: [AnalysisNet](https://github.com/m7nu3l/analysis-net.git), branch: cci-version-tinybct, revision: 6f80d3d16d4324. Their license file has been moved to src/lib/Daffodil.DatalogAnalysisFW/AnalysisNetBackend.

The pointer analysis code in src/datalog/CIPtrAnalysis.datalog started off with the base pointer analysis in [Chord](https://bitbucket.org/psl-lab/jchord/src/master/), removed the support for reflection, added support for pointers, delegates, and value types like structs. 


**Dependencies**
---
DAFFODIL has a compile-time dependency on Microsoft's CCI. The packages directory includes the publicly available nuget package for Microsoft.Cci.7.0.1.

DAFFODIL requires a Datalog solver. All the analysis files in src/datalog and the generated .datalog files conform to the input syntax required by the Z3 Datalog solver. This repository does not contain the Z3 binaries. They need to be installed separately.


**Download and Build**
---

Checkout DAFFODIL in some directory, say, <WORK_DIR>.

Execute dos2unix on all files in the src/datalog directory. This can be automated in any way: by using a .bat file, or a PowerShell script or a executing a bash script under Git bash.

Open <WORK_DIR>/DAFFODIL/DAFFODIL.sln in VisualStudio and build.


**Configuration Parameters**
---

All inputs required by DAFFODIL are given as configuration parameters in a config file named daffodil.cfg (the name of the config file can be anything). The configuration parameters are explained below. For syntax, please see: src/test/daffodil.cfg_sample (also, src/test/T4/daffodil.cfg).
   - AssemblyPath: Name of the assembly (i.e. the exe) to analyze.
   - DatalogDir: The directory where DAFFODIL will dump the generated program facts (referred to as <DATALOG_DIR>).
   - LogDir: The directory where DAFOODIL will dump debug logs (referred to as <LOG_DIR>).
   - StubsPath: The directory that contains the library stubs (to prevent the analysis from descending deep into system libraries in the interest of scalability). DAFFODIL contains a default stub implementation: C:\Users\xyz\DAFFODIL\src\stubs\Daffodil.Stubs\bin\Debug\Daffodil.Stubs.dll.
   - SaveScopePath: The directory where DAFFODIL will save the computed scope.
   - LoadSavedScope: This should currently be false because the Save/Load of scope is not working.
   - SuppressSystemExceptions: Can be true or false. If it is false, the pointer analysis will not track exception objects allocated in the system libraries.
   - SourceRoot: The root directory for the analyzed (application) source code. This parameter is not currently used; it is present for future use.


**Usage**
---
Perform the steps below to analyze a banchmark:
   - Create a config file daffodil.cfg for a benchmark that needs to be analyzed. Configuration options are explained above. NOTE: Directories <DATALOG_DIR> and <LOG_DIR> need to be already created.
   - Execute:

     `<WORK_DIR>\DAFFODIL\src\app\Daffodil.FactGeneratorSA\bin\Debug\Daffodil.FactGeneratorSA.exe daffodil.cfg`.

     Alternatively, the config file daffodil.cfg can be loaded in the main method of Daffodil.FactGeneratorSA and can be executed from within VisualStudio.
   - Execute: `cp <WORK_DIR>\DAFFODIL\src\datalog\* <DATALOG_DIR>`.
   - Execute: `. ./run_all.sh` in <DATALOG_DIR>.
       + I tar up the <DATALOG_DIR>, take it to a linux machine on which I have installed Z3 and execute run_all.sh on the linux machine. This is because Z3 consumes a large amount of memory to execute the pointer analysis on realistic benchmarks. Moreover, once the program to be analyzed is converted to a relational form, it is no longer Windows-specific.
       + The solver Z3 produces a text output that is a dump of all the facts of the output relations to stdout. The script src/datalog/parse_z3_out.py (see run_all.sh) is a python script to parse this output and populate the output relations.
       + Alternatively, instead of the file interface to Z3 implemented in DAFFODIL, a more efficient option will be to invoke Z3 programmatically using the library interfaces provided by Z3. 


**General Notes**
---

   - The Datalog solver Z3 complains about domains with zero size. This means that all the domains: EH, F, H, I, M, P, T, V, X and Z (please see src/datalog/CIPtrAnalysis.datalog for their meanings) must be of non-zero size. The smallest program that achieves this is:

```
public class MainCl
{
    public static void Main()
    {
        try
        {
            MainCl someObj = new MainCl();
        }
        catch {}
    }
}
```

   - It is safe to clear <LOG_DIR> and <DATALOG_DIR> everytime Datalog facts are regenerated in these directories because they are not automatically cleared before dumping data.

   - Save and Load scope is not working as of now because of a limitation of CCI.
