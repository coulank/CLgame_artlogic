using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coulank.Calc
{
    class ValueSet
    {
        public static int Max(int refValue, int setValue)
        {
            if (refValue < setValue) refValue = setValue;
            return refValue;
        }
        public static float Max(float refValue, float setValue)
        {
            if (refValue < setValue) refValue = setValue;
            return refValue;
        }
    }
}
