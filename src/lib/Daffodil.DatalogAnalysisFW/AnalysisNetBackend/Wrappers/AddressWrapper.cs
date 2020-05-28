// Author: Sulekha Kulkarni
// Date: Nov 2019


using Daffodil.DatalogAnalysisFW.ProgramFacts;

namespace Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers
{
    public enum AddressKind
    {
        AddrM,
        AddrHF,
        AddrF,
        AddrV
    }
    public class AddressWrapper : IWrapper
    {
        MethodRefWrapper mRefW;
        InstructionWrapper instW;
        FieldRefWrapper fldRefW;
        VariableWrapper varW;
        readonly AddressKind kind;


        public AddressWrapper(MethodRefWrapper mRefW)
        {
            this.mRefW = mRefW;
            kind = AddressKind.AddrM;
        }

        public AddressWrapper(InstructionWrapper instW, FieldRefWrapper fldRefW)
        {
            this.instW = instW;
            this.fldRefW = fldRefW;
            kind = AddressKind.AddrHF;
        }

        public AddressWrapper(FieldRefWrapper fldRefW)
        {
            this.fldRefW = fldRefW;
            kind = AddressKind.AddrF;
        }

        public AddressWrapper(VariableWrapper varW)
        {
            this.varW = varW;
            kind = AddressKind.AddrV;
        }
        public override string ToString()
        {
            if (kind == AddressKind.AddrM)
            {
                return mRefW.ToString();
            }
            else if (kind == AddressKind.AddrHF)
            {
                return (instW.ToString() + "::" + fldRefW.ToString());
            }
            else if (kind == AddressKind.AddrF)
            {
                return (fldRefW.ToString());
            }
            else if (kind == AddressKind.AddrV)
            {
                return (varW.ToString());
            }
            else
            {
                return "UNK";
            }
        }

        public string GetDesc()
        {
            if (kind == AddressKind.AddrM)
            {
                return mRefW.GetDesc();
            }
            else if (kind == AddressKind.AddrHF)
            {
                return instW.GetDesc() + "::" + fldRefW.GetDesc();
            }
            else if (kind == AddressKind.AddrF)
            {
                return (fldRefW.GetDesc());
            }
            else if (kind == AddressKind.AddrV)
            {
                return (varW.GetDesc());
            }
            else
            {
                return "UNK";
            }
        }
    }
}
