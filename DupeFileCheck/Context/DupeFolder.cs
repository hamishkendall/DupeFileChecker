using System;
using System.Collections.Generic;
using System.Text;

namespace DupeFileCheck.Context
{
    public class DupeFolder
    {
        public string Path { get; set; }
        public List<DupeFile> FileList { get; set; }
        public List<DupeFolder> SubFolderList { get; set; }
    }
}
