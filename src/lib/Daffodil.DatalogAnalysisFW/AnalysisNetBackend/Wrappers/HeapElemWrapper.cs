// Author: Sulekha Kulkarni
// Date: Nov 2019


using Daffodil.DatalogAnalysisFW.ProgramFacts;

namespace Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Wrappers
{
    public enum HeapElemKind
    {
        RefObj,
        StructObj,
        ArrElemStructObj,
        StatFldStructObj,
        InstFldStructObj
    }
    public class HeapElemWrapper : IWrapper
    {
        InstructionWrapper instW;
        VariableWrapper varW;
        FieldRefWrapper fldW;
        readonly HeapElemKind kind;


        public HeapElemWrapper(InstructionWrapper instW, bool createArrayElem)
        {
            this.instW = instW;
            kind = createArrayElem ? HeapElemKind.ArrElemStructObj : HeapElemKind.RefObj;
        }

        public HeapElemWrapper(VariableWrapper varW)
        {
            this.varW = varW;
            kind = HeapElemKind.StructObj;
        }

        public HeapElemWrapper(FieldRefWrapper fldW)
        {
            this.fldW = fldW;
            kind = HeapElemKind.StatFldStructObj;
        }

        public HeapElemWrapper(InstructionWrapper instW, FieldRefWrapper fldW)
        {
            this.instW = instW;
            this.fldW = fldW;
            kind = HeapElemKind.InstFldStructObj;
        }

        public override string ToString()
        {
            if (kind == HeapElemKind.RefObj)
            {
                return (instW.ToString());
            }
            else if (kind == HeapElemKind.StructObj)
            {
                return (varW.ToString());
            }
            else if (kind == HeapElemKind.ArrElemStructObj)
            {
                return (instW.ToString() + " ARRAY_ELEMENT");
            }
            else if (kind == HeapElemKind.StatFldStructObj)
            {
                return (fldW.ToString() + " STATIC_STRUCT_FLD");
            }
            else if (kind == HeapElemKind.InstFldStructObj)
            {
                return (instW.ToString() + "::" + fldW.ToString() + " INSTANCE_STRUCT_FLD");
            }
            else
            {
                return "UNK";
            }
        }

        public string GetDesc()
        {
            if (kind == HeapElemKind.RefObj)
            {
                return (instW.GetDesc());
            }
            else if (kind == HeapElemKind.StructObj)
            {
                return (varW.GetDesc());
            }
            else if (kind == HeapElemKind.ArrElemStructObj)
            {
                return (instW.GetDesc());
            }
            else if (kind == HeapElemKind.StatFldStructObj)
            {
                return (fldW.GetDesc());
            }
            else if (kind == HeapElemKind.InstFldStructObj)
            {
                return (instW.GetDesc() + " FLDDESC: " + fldW.GetDesc());
            }
            else
            {
                return "UNK";
            }
        }
    }
}

