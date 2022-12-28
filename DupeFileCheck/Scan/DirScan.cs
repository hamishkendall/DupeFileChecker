using DupeFileCheck.Context;
using DupeFileCheck.DirHash;
using DupeFileCheck.Logger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DupeFileCheck.Scan
{
    public class DirScan
    {
        #region Non-Parallel
        //Searches Dir and Sub Dir only keeping track of files in a single list
        public static List<DupeFile> SearchAllFiles(string dir)
        {
            List<DupeFile> retList = new List<DupeFile>();
            Search(dir, retList);
            return retList;
        }

        //Creates a list of all files within a directory
        private static void Search(string dir, List<DupeFile> files)
        {
            FileInfo curFile;

            try
            {
                foreach (string f in Directory.GetFiles(dir))
                {
                    curFile = new FileInfo(f);

                    if (IsFileReadable(f))
                    {
                        files.Add(new DupeFile()
                        {
                            Name = curFile.Name,
                            Path = curFile.FullName,
                            Extension = curFile.Extension,
                            Hash = CreateFileHash.GetHash(f)
                        });
                    }
                }

                foreach (string d in Directory.GetDirectories(dir))
                {
                    Search(d, files);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                CustomLogger.LogError(e);
            }
        }

        private static bool IsFileReadable(string filePath)
        {
            try
            {
                using(FileStream stream = File.OpenRead(filePath))
                {
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                CustomLogger.LogError(e);
                return false;
            }

            return true;
        }
        #endregion

        #region Parallel
        public static List<DupeFile> SearchAllFilesParallel(string dir)
        {
            ConcurrentBag<DupeFile> retList = new ConcurrentBag<DupeFile>();
            SearchParallel(dir, retList);
            return retList.ToList<DupeFile>();
        }

        private static void SearchParallel(string dir, ConcurrentBag<DupeFile> files)
        {
            try
            {
                Parallel.ForEach(Directory.GetFiles(dir), f =>
                {
                    FileInfo curFile = new FileInfo(f);

                    if (IsFileReadableParallel(f))
                    {
                        files.Add(new DupeFile()
                        {
                            Name = curFile.Name,
                            Path = curFile.FullName,
                            Extension = curFile.Extension,
                            Hash = CreateFileHash.GetHash(f)
                        });
                    }
                });

                Parallel.ForEach(Directory.GetDirectories(dir), d =>
                {
                    SearchParallel(d, files);
                });
            }
            catch (UnauthorizedAccessException e)
            {
                CustomLogger.LogError(e);
            }
        }
        
        private static bool IsFileReadableParallel(string filePath)
        {
            try
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                CustomLogger.LogError(e);
                return false;
            }

            return true;
        }
        #endregion
    }
}
