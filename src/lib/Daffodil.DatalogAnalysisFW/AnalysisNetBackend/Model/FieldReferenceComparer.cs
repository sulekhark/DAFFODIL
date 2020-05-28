using Microsoft.Cci;
using System.Collections.Generic;

namespace Daffodil.DatalogAnalysisFW.AnalysisNetBackend.Model
{
    class FieldReferenceComparer : IEqualityComparer<IFieldReference>
    {
        public int GetHashCode(IFieldReference fld)
        {
            return fld.InternedKey.GetHashCode();
        }

        public bool Equals(IFieldReference x, IFieldReference y)
        {
            bool result;
            var xdef = x as IFieldDefinition;
            var ydef = y as IFieldDefinition;

            if (xdef == null) xdef = x.ResolvedField;
            if (ydef == null) ydef = y.ResolvedField;
            result = (xdef.InternedKey == ydef.InternedKey);
            return result;
        }

        /******
        public bool Equals(IFieldReference x, IFieldReference y)
        {
            bool result;
            var xdef = x as IFieldDefinition;
            var ydef = y as IFieldDefinition;

            if (xdef != null && ydef == null)
            {
                result = y.ResolvedField.Equals(xdef);
            }
            else if (xdef == null && ydef != null)
            {
                result = x.ResolvedField.Equals(ydef);
            }
            else
            {
                result = x.Equals(y);
            }
            return result;
        }
        ******/
    }
}
