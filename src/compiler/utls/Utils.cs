using System.Runtime.CompilerServices;

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
            string s = File.ReadAllText(path);


            // NOTE: Very important check
            // due to some casts later on
            if (s.Length >= (int.MaxValue >> 2))
            {
                LogErr("Too Large file: " + path);
                return new Result<string>("", Res.Err);
            }

            // add padding nul chars
            var padding = new char[10];
            Array.Fill(padding, char.MinValue);

            return new Result<string>(s + padding);
        }

        LogErr("File [" + path + "] not found");
        return new Result<string>("", Res.Err);
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