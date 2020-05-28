using System.Collections.Generic;
using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.ThreeAddressCode.Instructions;

namespace Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Model
{
    class InstructionComparer : IEqualityComparer<Instruction>
    {
        public int GetHashCode(Instruction inst)
        {
            return inst.GetHashCode();
        }

        public bool Equals(Instruction x, Instruction y)
        {
            if (x == null || y == null) return false;
            return (x.Equals(y));
        }
    }
}
