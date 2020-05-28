// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System.Collections;
using System.Collections.Generic;

namespace Daffodil.DatalogAnalysisFW.Utils
{
    public class IndexMap<T> : IEnumerable<T>
    {
        /**
         * Implementation for indexing a set of objects by the order in which the objects are added to the set.
         * Maintains a nlist and a dictionary.
         * Provides constant-time operations for adding a given object, testing membership of a given object,
         * getting the index of a given object, and getting the object at a given index.
         * Provides O(1) access to the object at a given index by maintaining a list.
         * (NOTE: the documentation in https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.item?view=netframework-4.7.2 
         * says that "Retrieving the value of this property is an O(1) operation; setting the property is also an O(1) operation." )
         * Provides O(1) membership testing and access to the index of a given object by maintaining a dictionary.
         *
         * @param <T> The type of objects in the set.
         *
         */
        protected readonly List<T> list;
        protected readonly Dictionary<T, int> hmap;

        public IndexMap()
        {
            list = new List<T>();
            hmap = new Dictionary<T, int>();
        }

        public void Clear()
        {
            list.Clear();
            hmap.Clear();
        }

        public bool Contains(T val)
        {
            return hmap.ContainsKey(val);
        }

        public int IndexOf (T val)
        {
            int idx = -1;
            if (hmap.ContainsKey(val)) idx = hmap[val];
            return idx;
        }

        public int GetOrAdd(T val)
        {
            int idx;
            if (hmap.ContainsKey(val))
            {
                idx = hmap[val];
            }
            else
            {
                idx = list.Count;
                hmap[val] = idx;
                list.Add(val);
            }
            return idx;
        }

        public bool Add(T val)
        {
            if (hmap.ContainsKey(val))
            {
                return false;
            }
            else
            {
                hmap[val] = list.Count;
                list.Add(val);
                return true;
            }
        }

        public T GetVal(int idx)
        {
            if (idx >= 0 && idx < list.Count)
            {
                return list[idx];
            }
            else
            {
                throw new System.Exception("IndexMap: Index beyond list size.");
            }
        }

        public int Size()
        {
            return list.Count;
        }

        public bool AddAll(ICollection<T> collection)
        {
            bool result = false;
            foreach (T t in collection)
            {
                result |= Add(t);
            }
            return result;
        }
        public IEnumerator<T> GetEnumerator()
        {
            foreach (T val in list)
            {
                yield return val;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
