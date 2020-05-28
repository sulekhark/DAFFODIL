// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;
using System.IO;
using Daffodil.DatalogAnalysisFW.Utils;
using Daffodil.DatalogAnalysisFW.Common;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts
{
    public class Dom<T> : IndexMap<T> where T : IWrapper
    {
        protected readonly string name;

        public Dom(string dName)
        {
            name = dName;
        }

        public string GetName()
        {
            return name;
        }

        public void Save(string dirName, bool saveDomMap)
        {
            string mapFileName = "";
            int sz = Size();
            if (saveDomMap)
            {
                mapFileName = name + ".map";
                string mapPath = Path.Combine(dirName, mapFileName);
                using (StreamWriter swMap = new StreamWriter(mapPath))
                {
                    for (int i = 0; i < sz; i++)
                    {
                        T val = GetVal(i);
                        string printStr = ToUniqueString(val);
                        printStr = printStr.Replace("\n", "");
                        printStr = printStr.Replace("\r", "");
                        swMap.WriteLine(printStr + "  " + val.GetDesc());
                    }
                }
            }
            string domFileName = name + ".dom";
            string domPath = Path.Combine(dirName, domFileName);

            using (StreamWriter swDom = new StreamWriter(domPath))
            {
                swDom.WriteLine(name + " " + sz);
            }

            // Create a dummy relation for each dom (containing all the dom elements).
            // This is to make Z3 assign the same dom ids as we do.
            // Most likely this is required because of using Z3 via the commandline interface.
            string relName = "rel" + name;
            string relFileName = relName + ".datalog";
            string relPath = Path.Combine(dirName, relFileName);
            using (StreamWriter swRel = new StreamWriter(relPath))
            {
                for (int i = 0; i < sz; i++) swRel.WriteLine(relName + "(" + i + ").");
            }
        }

        public void Save()
        {
            string outDir = ConfigParams.DatalogDir;
            Save(outDir, true);
        }

        public string ToUniqueString(T val)
        {
            return val == null ? "null" : val.ToString();
        }

        public string ToUniqueString(int idx)
        {
            T val = GetVal(idx);
            return ToUniqueString(val);
        }

        public void Print(StreamWriter outSw)
        {
            for (int i = 0; i < Size(); i++)
            {
                outSw.WriteLine(GetVal(i));
            }
        }

        public void Print()
        {
            Print(new StreamWriter(Console.OpenStandardOutput()));
        }

        public override bool Equals(object o)
        {
            return this == o;
        }

        // Overriding public int GetHashCode() trivially because Object.GetHashCode
        // gives a pretty reliable identifier (though not 100%).
        // If required at a later point in time, can use ObjectIdGenerator
        // This will keep a reference to the object alive and hence the garbage collector
        // cannot remove it. So, it suits objects that last the application's 
        // lifetime but not ephemeral objects.
        // Or, look at ObjectHashValue in SystemController/Logging

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
