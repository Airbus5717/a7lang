using System.Runtime.CompilerServices;
using System.Diagnostics;

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
    private static readonly string[] LOG_STRINGS = {
        "[ERROR]: ", "[INFO] : ", "[WARN] : ", "[TIME] : "
    };

    public const int NULL_TERMINATORS_COUNT_FILE_READ = 10;
    public const int NULL_TERMINATORS_COUNT_PASSES = 5;

    public static void LogErr(string s)
        => Log(ConsoleColor.Red, LOG_STRINGS[0], s);

    public static void LogInfo(string s)
        => Log(ConsoleColor.Green, LOG_STRINGS[1], s);


    public static void LogWarn(string s)
        => Log(ConsoleColor.Yellow, LOG_STRINGS[2], s);

    public static void LogTime(string s)
        => Log(ConsoleColor.Magenta, LOG_STRINGS[3], s);

    public static void Log(ConsoleColor c, string i, string s)
    {
        Console.ForegroundColor = c;
        Console.Write(i);
        Console.ResetColor();
        Console.WriteLine(s);
    }

    public static Result<string> ReadFile(string path)
    {
        if (Path.Exists(path))
        {
            string s = "";
            try
            {
                // NOTE: Exceptions are considered a bad practice
                s = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                LogErr(e.Message);
                return new Result<string>("", Res.Err);
            }

            //! NOTE: Very important check
            if (s.Length >= (int.MaxValue >> 2))
            {

                LogErr("Too Large file: " + path);
                return new Result<string>(s, Res.Err);
            }

            // add padding nul chars
            return new Result<string>(PrepareStrForParsing(s));
        }

        LogErr("File [" + path + "] not found");
        return new Result<string>("", Res.Err);
    }

    public static string PrepareStrForParsing(string s)
    {
        // add newline + '\0' s
        char[] padding = new char[NULL_TERMINATORS_COUNT_FILE_READ];
        Array.Fill(padding, char.MinValue);
        s = s + '\n' + new string(padding);
        return s;
    }

    public static void Exit(int x)
        => Environment.Exit(x);


    public static void Assert(bool expr)
        => Debug.Assert(expr);


    public static void Todo(string text,
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

    public static void UNREACHABLE(
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[Unreachable] : reached unreachable state");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("file: {0}, line: {1}, method: {2}", file, line, member);
        Console.ResetColor();
        PrintStackTrace();
        Exit(1);
    }

    private static void PrintStackTrace()
    {
        Console.WriteLine(new System.Diagnostics.StackTrace().ToString());
    }
}
