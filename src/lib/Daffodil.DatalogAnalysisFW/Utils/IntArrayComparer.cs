// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System.Collections.Generic;

namespace Daffodil.DatalogAnalysisFW.Utils
{
    public class IntArrayComparer : IEqualityComparer<int[]>
    {
        public int GetHashCode(int[] arr)
        {
            int hc = arr.Length;
            for (int i = 0; i < arr.Length; ++i)
            {
                hc = unchecked(hc * 31 + arr[i]);
            }
            return hc;
        }

        public bool Equals(int[] arr1, int[] arr2)
        {
            int sz1 = arr1.Length;
            int sz2 = arr2.Length;
            if (sz1 != sz2) return false;
            for (int i = 0; i < sz1; i++)
            {
                if (arr1[i] != arr2[i]) return false;
            }
            return true;
        }
    }
}
