
using System.Runtime.CompilerServices;

public enum Status : byte
{
    Success,
    Failure,
    Done,
}


public class Utils
{
    // TODO: use a stdlib logger
    private static readonly string[] logStrings = {
        "[ERROR]: ", "[INFO] : ", "[WARN] : ", "[TIME] : "
    };

    public static void LogErr(string s)
    {
        Log(ConsoleColor.Red, 0, s);
    }

    public static void LogInfo(string s)
    {
        Log(ConsoleColor.Green, 1, s);
    }

    public static void LogWarn(string s)
    {
        Log(ConsoleColor.Yellow, 2, s);
    }

    public static void LogTime(string s)
    {
        Log(ConsoleColor.Magenta, 3, s);
    }

    private static void Log(ConsoleColor c, int i, string s)
    {
        Console.ForegroundColor = c;
        Console.Write(logStrings[i]);
        Console.ResetColor();
        Console.WriteLine(s);
    }


    public static string? ReadFile(string path)
    {
        if (Path.Exists(path))
        {
            string s = File.ReadAllText(path);


            // NOTE: Very important check
            if (s.Length >= int.MaxValue)
            {
                LogErr("Too Large file: " + path);
                return null;
            }

            // add padding nul chars
            var padding = new char[10];
            Array.Fill(padding, char.MinValue);

            return s + padding;
        }

        LogErr("File [" + path + "] not found");
        return null;
    }

    public static void Exit(int x)
    {
        Environment.Exit(x);
    }

    public static void TODO(string text,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[TODO] : " + text);
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("file: {0}, line: {1}, method: {2}", file, line, member);
        Console.ResetColor();
        Exit(1);
    }
}