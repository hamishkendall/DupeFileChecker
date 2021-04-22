using DupeFileCheck.Context;
using DupeFileCheck.DirHash;
using DupeFileCheck.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DupeFileCheck.Scan
{
    public class DirScan
    {
        //Searches Dir and Sub Dir only keeping track of files in a single list
        public static List<DupeFile> SearchAllFiles(string dir)
        {
            List<DupeFile> retList = new List<DupeFile>();
            Search(dir, retList);
            return retList;
        }
        private static void Search(string dir, List<DupeFile> files)
        {
            try
            {
                foreach (string f in Directory.GetFiles(dir))
                {
                    files.Add(new DupeFile()
                    {
                        Name = f.Replace($@"{dir}{Path.DirectorySeparatorChar}", ""),
                        Path = f,
                        Hash = CreateFileHash.GetHash(f)
                    });
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


        //Searches the Dir and Sub Dir creating a file list for each dir
        public static DupeTree SearchDirAndSubDirTree(string rootDir)
        {
            DupeTree tree = new DupeTree();
            Console.WriteLine("Starting Scan");
            SearchTreeKeepStructure(rootDir, tree);
            return tree;
        }
        private static void SearchTreeKeepStructure(string dir, DupeTree tree)
        {
            foreach (string f in Directory.GetFiles(dir))
            {
                tree.Files.Add(new DupeFile()
                {
                    Name = f.Replace(dir, ""),
                    Path = f
                    //Hash = CreateFileHash.GetHash(f)
                }) ;
            }
            Console.WriteLine($"Added {tree.Files.Count} Root Files from {dir}");

            foreach (string d in Directory.GetDirectories(dir))
            {
                tree.Folders.Add(new DupeFolder()
                {
                    Path = d,
                    FileList = GetSubDirFiles(d),
                    SubFolderList = GetSubDirFolders(d)
                });
            }
            Console.WriteLine($"Added {tree.Folders.Count} Root Folders from {dir}");
        }
        private static List<DupeFile> GetSubDirFiles(string dir)
        {
            List<DupeFile> retFiles = new List<DupeFile>();
            try
            {
                foreach(string f in Directory.GetFiles(dir))
                {
                    Console.WriteLine($"Added File {f}");
                    retFiles.Add(new DupeFile()
                    {
                        Name = f.Replace(dir + @"\", ""),
                        Path = f
                        //Hash = CreateFileHash.GetHash(f)
                    });
                }
            }
            catch(UnauthorizedAccessException e)
            {
                CustomLogger.LogError(e);
            }
            Console.WriteLine($"Added {retFiles.Count} Sub Files from {dir}");
            return retFiles;
        }
        private static List<DupeFolder> GetSubDirFolders(string dir)
        {
            List<DupeFolder> retFolders = new List<DupeFolder>();
            try
            {
                foreach(string d in Directory.GetDirectories(dir))
                {
                    Console.WriteLine($"Added Folder {d}");
                    retFolders.Add(new DupeFolder()
                    {
                        Path = d,
                        FileList = GetSubDirFiles(d),
                        SubFolderList = GetSubDirFolders(d)
                    });
                }
            }
            catch(UnauthorizedAccessException e)
            {
                CustomLogger.LogError(e);
            }
            Console.WriteLine($"Added {retFolders.Count} Sub Folders in {dir}");
            return retFolders;
        }

    }
}
