using System;
using System.IO;

namespace DupeFileCheck.Logger
{
    public class CustomLogger
    {
        private static readonly string _path = Environment.CurrentDirectory + @"/Logs/";
        private static readonly string _name = DateTime.Now.ToFileTimeUtc()+".txt";
        private static readonly string _fullPath = Path.Combine(_path, _name);

        private static readonly object _lock = new object();

        public static void LogError(Exception e)
        {
            lock (_lock)
            {
                CheckLogFile();

                using (StreamWriter w = File.AppendText(_fullPath))
                {
                    WriteMessageToConsole("Error", $"{e.GetType()} : {e.Message}");
                    w.Write("\r\nError : ");
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} ~ {DateTime.Now.ToLongDateString()}");
                    w.WriteLine($"{e.GetType()} : {e.Message}");
                    w.WriteLine("-------------------------------");
                }
            }
        }

        public static void LogMessage(string msg)
        {
            lock (_lock)
            {
                CheckLogFile();

                using (StreamWriter w = File.AppendText(_fullPath))
                {
                    WriteMessageToConsole("Message", $"{msg}");
                    w.Write("\r\nMessage : ");
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} ~ {DateTime.Now.ToLongDateString()}");
                    w.WriteLine(msg);
                    w.WriteLine("-------------------------------");
                }
            }
        }
    
        private static void CheckLogFile()
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            if (!File.Exists(_fullPath))
            {
                var file = File.Create(_fullPath);
                file.Close();
            }
        }

        private static void WriteMessageToConsole(string msgType, string msg)
        {
            lock (_lock)
            {
                Console.WriteLine("\r\n-------------------------------");
                Console.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                Console.WriteLine($"{msgType} : {msg}");
                Console.WriteLine("-------------------------------");
            }
        }
    }
}
