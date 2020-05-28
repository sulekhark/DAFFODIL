// Author: Sulekha Kulkarni
// Date: Nov 2019


using Microsoft.Cci;
using Daffodil.DatalogAnalysisFW.AnalysisNetConsole;
using Daffodil.DatalogAnalysisFW.ProgramFacts;



namespace Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers
{
    public class FieldRefWrapper : IWrapper
    {
        readonly IFieldReference fld;
        readonly string typeName;

        public FieldRefWrapper(IFieldReference fld)
        {
            this.fld = fld;
            typeName = (fld == null) ? "" : fld.ContainingType.FullName();
        }

        public override string ToString()
        {
            if (fld == null)
            {
                return "null";
            }
            else
            {
                return fld.Name.Value;
            }
        }

        public string GetDesc()
        {
            string s = "CLASS:" + typeName;
            return s;
        }
    }
}
