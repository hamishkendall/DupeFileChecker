using DupeFileCheck.Context;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DupeFileCheck.Hash
{
    public class CompareFileHash
    {
        #region Non-Parallel
        public static Dictionary<string, List<DupeFile>> FindHashDuplicates(List<DupeFile> fileList)
        {
            Dictionary<string, List<DupeFile>> dupeDict = new Dictionary<string, List<DupeFile>>();

            string curHash;
            List<DupeFile> curCheckList;

            for (int i = 0; i < fileList.Count-1; ++i)
            {
                curHash = fileList[i].Hash;
                curCheckList = ScanForValue(curHash, fileList.GetRange(i, fileList.Count - i));

                if(curCheckList.Count > 1)
                {
                    if (!dupeDict.ContainsKey(curHash))
                        dupeDict.Add(curHash, new List<DupeFile>());

                    foreach (DupeFile d in curCheckList)
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
            foreach (DupeFile d in searchList)
            {
                if (d.Hash.Equals(hash))
                    matches.Add(d);
            }
            return matches;
        }
        #endregion

        #region Parallel
        public static Dictionary<string, ConcurrentBag<DupeFile>> FindHashDuplicatesParallel(List<DupeFile> fileList)
        {
            ConcurrentDictionary<string, ConcurrentBag<DupeFile>> dupeConcurrDict = new ConcurrentDictionary<string, ConcurrentBag<DupeFile>>();

            Parallel.For(0, fileList.Count - 1, i =>
            {
                string curHash = fileList[i].Hash;
                ConcurrentBag<DupeFile> curCheckList = ScanForValueParallel(curHash, fileList.GetRange(i, fileList.Count - i));

                if (curCheckList.Count > 1)
                {
                    if (!dupeConcurrDict.ContainsKey(curHash))
                        dupeConcurrDict.TryAdd(curHash, new ConcurrentBag<DupeFile>());

                    foreach (DupeFile d in curCheckList)
                    {
                        if (!dupeConcurrDict[curHash].Contains(d))
                            dupeConcurrDict[curHash].Add(d);
                    }
                }
            });

            return dupeConcurrDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private static ConcurrentBag<DupeFile> ScanForValueParallel(string hash, List<DupeFile> searchList)
        {
            ConcurrentBag<DupeFile> matches = new ConcurrentBag<DupeFile>();
            Parallel.ForEach(searchList, d =>
            {
                if (d.Hash.Equals(hash))
                    matches.Add(d);
            });
            return matches;
        }
        #endregion
    }
}
