using DupeFileCheck.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DupeFileCheck.Hash
{
    public class CompareFileHash
    {
        public static Dictionary<string, List<DupeFile>> FindHashDuplicates(List<DupeFile> fileList)
        {
            Dictionary<string, List<DupeFile>> dupeDict = new Dictionary<string, List<DupeFile>>();

            string curHash;
            List<DupeFile> dupeFileList;

            for (int i = 0; i < fileList.Count-1; ++i)
            {
                curHash = fileList[i].Hash;
                dupeFileList = ScanForValue(curHash, fileList.GetRange(i, fileList.Count - i));

                if(dupeFileList.Count > 1)
                {
                    if (!dupeDict.ContainsKey(curHash))
                        dupeDict.Add(curHash, new List<DupeFile>());

                    foreach (DupeFile d in dupeFileList)
                    {
                        if (!dupeDict[curHash].Contains(d))
                            dupeDict[curHash].Add(d);                            
                    }
                }
            }
            return dupeDict;
        }

        private static List<DupeFile> ScanForValue(string hash, List<DupeFile> searchList)
        {
            List<DupeFile> matches = new List<DupeFile>();
            foreach(DupeFile d in searchList)
            {
                if (d.Hash.Equals(hash))
                    matches.Add(d);
            }
            return matches;
        }
    }
}
