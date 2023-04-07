using System.Runtime.CompilerServices;
using System.Diagnostics;
using A7;

public enum Status : byte
{
    Success,
    Failure,
    Done,
}

public enum Res : byte
{
    Ok,
    Err,
}

public class Result<T>
{
    public T m_item;
    public Res m_res;

    public Result(T item, Res res = Res.Ok)
    {
        m_item = item;
        m_res = res;
    }
}

public class Utils
{
    // TODO: use a stdlib logger
    private static readonly string[] m_logStrings = {
        "[ERROR]: ", "[INFO] : ", "[WARN] : ", "[TIME] : "
    };

    public static int NULL_TERMINATORS_COUNT_FILE_READ = 10;
    public static int NULL_TERMINATORS_COUNT_PASSES = 5;

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
        Console.Write(m_logStrings[i]);
        Console.ResetColor();
        Console.WriteLine(s);
    }

    public static Result<string> ReadFile(string path)
    {
        if (Path.Exists(path))
        {
            string s;
            try
            {
                //! Exception is considered a bad practice
                s = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                LogErr(e.Message);
                return new Result<string>("", Res.Err);
            }
            // NOTE: Very important check
            // due to some casts later on
            if (s.Length >= (int.MaxValue >> 2))
            {
                LogErr("Too Large file: " + path);
                return new Result<string>("", Res.Err);
            }

            // add padding nul chars


            return new Result<string>(prepareStrForParsing(s));
        }

        LogErr("File [" + path + "] not found");
        return new Result<string>("", Res.Err);
    }

    public static string prepareStrForParsing(string s)
    {
        char[] padding = new char[NULL_TERMINATORS_COUNT_FILE_READ];
        Array.Fill(padding, char.MinValue);
        s = s + '\n' + new string(padding);
        return s;
    }

    public static void Exit(int x)
    {
        Environment.Exit(x);
    }


    public static void assert(bool expr)
    {
        Debug.Assert(expr);
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
