// Author: Sulekha Kulkarni
// Date: Nov 2019


using Daffodil.DatalogAnalysisFW.ProgramFacts;

namespace Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers
{
    public class IntWrapper : IWrapper
    {
        int val;

        public IntWrapper(int n)
        {
            val = n;
        }

        public override string ToString()
        {
            return val.ToString();
        }

        public string GetDesc()
        {
            return "";
        }
    }
}
