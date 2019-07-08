using System;
using System.IO;

public class Log
{
    static string t = "";
    static string level = "";
    public static void log_tab(string s)
    {
        log(level + s);
    }
    public static void log(string s)
    {
        t += s + "\r\n";
        Console.WriteLine(s);
    }
    public static void Save(string path)
    {
        File.WriteAllText(path, t);
    }

}




