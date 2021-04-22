using System;
using System.Collections.Generic;
using System.Text;

namespace DupeFileCheck.Context
{
    public class DupeTree
    {
        public List<DupeFile> Files;
        public List<DupeFolder> Folders;

        public DupeTree()
        {
            Files = new List<DupeFile>();
            Folders = new List<DupeFolder>();
        }

        public void ListAllFilesAndFolders()
        {
            Console.WriteLine("Files in Root:");
         
            foreach(DupeFile f in Files)
            {
                Console.Write(f.Name + " - ");
            }
            Console.WriteLine("");

            foreach(DupeFolder d in Folders)
            {
                ListSubFilesFolders(d);
            }

        }

        private void ListSubFilesFolders(DupeFolder dir)
        {
            Console.WriteLine($"Files in {dir.Path}:");

            foreach (DupeFile f in dir.FileList)
            {
                Console.Write(f.Name + " - ");
            }
            Console.WriteLine("");

            foreach(DupeFolder d in dir.SubFolderList)
            {
                ListSubFilesFolders(d);
            }
        }
    }
}
