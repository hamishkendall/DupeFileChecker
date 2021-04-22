using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DupeFileCheck.Context
{
    public class DupeFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Hash { get; set; }
    }
}
