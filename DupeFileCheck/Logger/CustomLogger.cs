using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DupeFileCheck.Logger
{
    public class CustomLogger
    {
        private static readonly string _path = Environment.CurrentDirectory + @"/Logs/";
        private static readonly string _name = DateTime.Now.ToFileTimeUtc()+".txt";

        public static void LogError(Exception e)
        {
            string fullPath = Path.Combine(_path, _name);

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            if(!File.Exists(fullPath))
            {
                var file = File.Create(fullPath);
                file.Close();
            }

            using (StreamWriter w = File.AppendText(fullPath))
            {
                WriteMessageToConsole("Error", $"{e.GetType()} : {e.Message}");
                w.Write("\r\nError : ");
                w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                w.WriteLine($"{e.GetType()} : {e.Message}");
                w.WriteLine("-------------------------------");
            }
        }

        public static void LogMessage(string msg)
        {
            string fullPath = Path.Combine(_path, _name);

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            if (!File.Exists(fullPath))
            {
                var file = File.Create(fullPath);
                file.Close();
            }

            using (StreamWriter w = File.AppendText(fullPath))
            {
                WriteMessageToConsole("Message", $"{msg}");
                w.Write("\r\nMessage : ");
                w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                w.WriteLine(msg);
                w.WriteLine("-------------------------------");
            }
        }
    
        private static void WriteMessageToConsole(string msgType, string msg)
        {
            Console.WriteLine("\r\n-------------------------------");
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            Console.WriteLine($"{msgType} : {msg}");
            Console.WriteLine("-------------------------------");
        }
    }
}
