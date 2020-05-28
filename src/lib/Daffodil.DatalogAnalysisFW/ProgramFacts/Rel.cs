// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;
using System.IO;
using System.Text;
using Daffodil.DatalogAnalysisFW.Utils;
using Daffodil.DatalogAnalysisFW.Common;

namespace Daffodil.DatalogAnalysisFW.ProgramFacts
{
    public class Rel : ArraySet
    {
        protected readonly string name;
        protected readonly int numDoms;
        protected string[] domNames;

        public Rel(int sz, string relName) : base(sz)
        {
            name = relName;
            numDoms = sz;
        }

        public string GetName()
        {
            return name;
        }

        public bool Load(string fileName)
        {
            throw new NotImplementedException();
        }

        public int GetNumDoms()
        {
            return numDoms;
        }

        public void Save(string dirName)
        {
            string relFileName = "";
            int sz = Size();

            relFileName = name + ".datalog";
            string relPath = Path.Combine(dirName, relFileName);
            using (StreamWriter sw = new StreamWriter(relPath))
            {
                foreach (int[] arr in arrSet)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(name);
                    sb.Append("(");
                    for (int i = 0; i < numDoms; i++)
                    {
                        sb.Append(arr[i].ToString());
                        if (i < numDoms - 1)
                        {
                            sb.Append(",");
                        }
                    }
                    sb.Append(").");
                    sw.WriteLine(sb.ToString());
                }
            }
        }

        public void Save()
        {
            string outDir = ConfigParams.DatalogDir;
            Save(outDir);
            Print(outDir); // SRK_DBG: for debugging only.
        }

        public void Print()
        {
            string outDir = ConfigParams.DatalogDir;
            Print(outDir);
        }

        public void Print(string dirName)
        {
            string relFileName = "";
            int sz = Size();

            relFileName = name + ".txt";
            string relPath = Path.Combine(dirName, relFileName);
            using (StreamWriter sw = new StreamWriter(relPath))
            {
                foreach (int[] arr in arrSet)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(name);
                    sb.Append("(");
                    for (int i = 0; i < numDoms; i++)
                    {
                        sb.Append(arr[i].ToString());
                        sb.Append(":");
                        string domName = domNames[i];
                        string uStr = ProgramDoms.DomUniqueString(domName, arr[i]);
                        sb.Append(uStr);
                        if (i < numDoms - 1) sb.Append(",   ");
                    }
                    sb.Append(").");
                    sw.WriteLine(sb.ToString());
                }
            }
        }
    }
}
