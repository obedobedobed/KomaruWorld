using System.Collections.Generic;
using System.IO;
using System;

namespace KomaruWorld;

public static class Logger
{
    private static List<string> logs = new List<string>();

    public static void Log(string msg)
    {
        msg = $"[L] > {msg}\n";
        logs.Add(msg);
        Console.Write(msg);
    }

    public static void Warning(string msg)
    {
        msg = $"[W] > {msg}\n";
        logs.Add(msg);
        Console.Write(msg);
    }

    public static void Error(string msg)
    {
        msg = $"[E] > {msg}\n";
        logs.Add(msg);
        Console.Write(msg);
    }

    public static void WriteLogs()
    {
        File.WriteAllText("Logs.txt", string.Empty);

        foreach (var str in logs)
            File.AppendAllText("Logs.txt", str);
    }
}