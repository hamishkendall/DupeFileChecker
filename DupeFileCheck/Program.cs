using DupeFileCheck.Context;
using DupeFileCheck.Hash;
using DupeFileCheck.Logger;
using DupeFileCheck.Scan;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DupeFileCheck
{
    public class Program
    {

        static void Main(string[] args)
        {
            string path = @"C:\Test\";
            SearchAllFiles(path);
        }

        private static void SearchAllFiles(string searchPath)
        {
            CustomLogger.LogMessage($"Starting File Scan in {searchPath}");
            List<DupeFile> allFiles = DirScan.SearchAllFiles(searchPath);

            CustomLogger.LogMessage($"File Scan Finished. Found {allFiles.Count} files - Starting Duplicate Check...");
            Dictionary<string, List<DupeFile>> hashMatches = CompareFileHash.FindHashDuplicates(allFiles);

            CustomLogger.LogMessage($"Compared {allFiles.Count} files - {hashMatches.Count} matches found!");

            CustomLogger.LogMessage("Creating Results File...");
            string resultsPath = CreateResultsFile();

            CustomLogger.LogMessage("Outputting results...");
            using (StreamWriter file = File.CreateText(resultsPath))
            {
                file.WriteLine(JsonConvert.SerializeObject(hashMatches, Formatting.Indented));
            }

            CustomLogger.LogMessage($"Finished. Results located at: {resultsPath}");
        }

        private static string CreateResultsFile()
        {
            string resultPath = Environment.CurrentDirectory + @"/Results/";
            string fullPath = Path.Combine(resultPath, "DupeResults.json");

            if (!Directory.Exists(resultPath))
                Directory.CreateDirectory(resultPath);

            if (!File.Exists(fullPath))
            {
                var file = File.Create(fullPath);
                file.Close();
            }

            return fullPath;
        }

        private static void SearchFilesFolderTree(string searchPath)
        {
            DupeTree tree = DirScan.SearchDirAndSubDirTree(searchPath);

            string json = JsonConvert.SerializeObject(tree, Formatting.Indented);
            using (StreamWriter file = File.CreateText(@"C:\test.json"))
            {
                file.WriteLine(json);
            }
        }
    }
}
