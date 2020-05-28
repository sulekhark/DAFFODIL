using System.Collections.Generic;
using Daffodil.DatalogAnalysisFW.AnalysisNetBackend.ThreeAddressCode.Values;

namespace Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Model
{
    class VariableComparer : IEqualityComparer<IVariable>
    {
        public int GetHashCode(IVariable var)
        {
            return var.GetHashCode();
        }

        public bool Equals(IVariable x, IVariable y)
        {
            if (x == null || y == null) return false;
            return (x.Equals(y));
        }
    }
}
