using DupeFileCheck.Context;
using DupeFileCheck.Hash;
using DupeFileCheck.Logger;
using DupeFileCheck.Scan;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace DupeFileCheck
{
    public class Program
    {
        #region Launch
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                HandleDefaultLaunch();
            }
            else
            {
                return HandleArgsLaunch(args);
            }

            return 0;
        }

        private static void HandleDefaultLaunch()
        {
            string path = ValidateUserFilePath();
            bool useParallel = ValidateUserParallel();

            StartSearch(path, useParallel);
        }

        private static int HandleArgsLaunch(string[] args)
        {
            if(args.Length != 2) {
                CustomLogger.LogError(new ArgumentException($"Requires 2 args - Folder path (string) & Use Parallel (Y / N)"));
                return 1;
            }

            if (!IsValidPath(args[0]))
            {
                CustomLogger.LogError(new ArgumentException($"{args[0]} is an invalid folder path"));
                return 1;
            }

            if (!IsValidParallel(args[1]))
            {
                CustomLogger.LogError(new ArgumentException($"Invalid input: {args[1]}"));
                return 1;
            }

            string path = args[0];
            bool useParallel = YesNoToBool(args[1]);


            StartSearch(path, useParallel);

            return 0;
        }

        private static void StartSearch(string path, bool useParallel)
        {
            if (useParallel)
            {
                CustomLogger.LogMessage($"Starting File Scan in {path} using parallel");
                SearchAllFilesParallel(path);
            }
            else
            {
                CustomLogger.LogMessage($"Starting File Scan in {path} without using parallel");
                SearchAllFiles(path);
            }
        }
        #endregion

        #region Validate
        private static string ValidateUserFilePath()
        {
            bool isValid = false;
            string path = "";

            while (!isValid)
            {
                Console.WriteLine("Please enter a valid filepath to scan:");
                path = Console.ReadLine();

                isValid = IsValidPath(path);
            }

            return path;
        }

        private static bool IsValidPath(string path)
        {
            if(!Directory.Exists(path))
                return false;

            return true;
        }

        private static bool ValidateUserParallel()
        {
            bool isValid = false;
            string userInput = "";

            while (!isValid)
            {
                Console.WriteLine("Using Parallel? Y or N");
                userInput = Console.ReadLine().ToUpper();

                isValid = IsValidParallel(userInput);
            }

            return YesNoToBool(userInput);
        }

        private static bool IsValidParallel(string userInput)
        {
            if (userInput.ToUpper().Equals("Y") || userInput.ToUpper().Equals("N"))
                return true;

            return false;
        }

        private static bool YesNoToBool(string letter)
        {
            if (letter.ToUpper().Equals("Y")) return true;

            return false;
        }
        
        #endregion

        #region Search
        private static void SearchAllFiles(string searchPath)
        {
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

        private static void SearchAllFilesParallel(string searchPath)
        {
            List<DupeFile> allFiles = DirScan.SearchAllFilesParallel(searchPath);

            CustomLogger.LogMessage($"File Scan Finished. Found {allFiles.Count} files - Starting Duplicate Check...");
            Dictionary<string, ConcurrentBag<DupeFile>> hashMatches = CompareFileHash.FindHashDuplicatesParallel(allFiles);
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
            string resultFolderPath = Path.Combine(Environment.CurrentDirectory, "Results");
            string resultFileName = $"{DateTime.Now.ToString("dd.M.yy_hhmmss")}.json";

            string resultFilePath = Path.Combine(resultFolderPath, resultFileName);

            if (!Directory.Exists(resultFolderPath))
                Directory.CreateDirectory(resultFolderPath);

            if (!File.Exists(resultFilePath))
            {
                var file = File.Create(resultFilePath);
                file.Close();
            }

            return resultFilePath;
        }
        #endregion
    }
}
