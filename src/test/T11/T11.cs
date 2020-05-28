// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿namespace T11
{
    class T11
    {
        static void Main(string[] args)
        {
            MyDict<string, MyObj> dict;
            dict = new MyDict<string, MyObj>();
            MyObj sampleObj = new MyObj();
            dict.Add("aaa", sampleObj);
            MyObj xxx = dict["aaa"];
        }
    }

    class MyObj { }

    class MyDict<TKey, TValue>
    {
        struct entry
        {
            public TKey key;
            public TValue value;
        }

        entry[] entries;
        int counter;

        public MyDict()
        {
            entries = new entry[10];
            counter = 0;
        }

        public void Add(TKey key, TValue val)
        {
            entries[counter].key = key;
            entries[counter].value = val;
            counter++;
        }

        public TValue this[TKey key]
        {
            get
            {
                for (int i = 0; i < 10; i++)
                {
                    if (entries[i].key.Equals(key)) return entries[i].value;
                }
                return default(TValue);
            }
        }

    }
}
