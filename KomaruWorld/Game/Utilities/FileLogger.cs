using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KomaruWorld
{
    public static class FileLogger
    {
        // Default to a generic name, but Initialize will change it
        private static string _debugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "generic.log");
        private static object _lock = new object();

        public static event Action<string> OnLogReceived;

        public static void Initialize(string role)
        {
            lock (_lock)
            {
                // CHANGE: Use the role to create a unique filename (server.log vs client.log)
                _debugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{role.ToLower()}.log");

                string msg = $"=== DEBUG SESSION STARTED: {DateTime.Now} ===";
                try 
                {
                    File.WriteAllText(_debugPath, msg + "\n");
                }
                catch { /* Handle file in use errors gracefully */ }
            }
        }

        public static void Log(string message)
        {
            lock (_lock)
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                string line = $"[{timestamp}] {message}";

                // 1. Invoke Event FIRST (Updates Console/UI even if disk fails)
                OnLogReceived?.Invoke(line);

                // 2. Write to Disk
                try
                {
                    File.AppendAllText(_debugPath, line + "\n");
                }
                catch { /* Ignore file access errors */ }
            }
        }

        public static List<string> GetTail(int lineCount)
        {
            lock (_lock)
            {
                if (!File.Exists(_debugPath)) return new List<string> { "Log file not found." };
                try
                {
                    string[] allLines = File.ReadAllLines(_debugPath);
                    return allLines.Reverse().Take(lineCount).Reverse().ToList();
                }
                catch (Exception ex)
                {
                    return new List<string> { $"Error reading log: {ex.Message}" };
                }
            }
        }

        public static void LogThrottled(string key, string message, double t, double i) 
        { 
            // Optional implementation
        }
    }
}