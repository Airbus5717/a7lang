using A7;

public enum ErrKind
{
    // LEXER
    UNKNOWN,
    STR_NOT_CLOSED,
    NUM_TOO_LONG,

    // PARSER
}

public enum Stage
{
    READ_FILE,
    LEXER,
    PARSER,
    TYPE_CHECK,
}


public class Err
{

    public static string GetErrString(ErrKind kind)
    {
        switch (kind)
        {
            case ErrKind.UNKNOWN: return "Unknown";
            case ErrKind.STR_NOT_CLOSED: return "String not closed";
            case ErrKind.NUM_TOO_LONG: return "Number literal is too long";
            default: break;
        }
        Utils.Todo("Add an Error string for " + kind.ToString());
        return "";
    }

    public static string GetStageString(Stage stage)
    {
        switch (stage)
        {
            case Stage.READ_FILE: return "READ FILE";
            case Stage.LEXER: return "LEXER";
            case Stage.PARSER: return "PARSER";
            case Stage.TYPE_CHECK: return "TYPE CHECK";
            default: break;
        }
        Utils.Todo("Add an Stage string for " + stage.ToString());
        return "";
    }

    public static void LexerErrMsg(Lexer l)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("> File: {0}:{1}:", l.filename, l.GetLine());
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(" error: ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(l.GetErr());
        Console.ResetColor();
        Console.WriteLine(" {0} | {1}", l.GetLine(), l.m_file.Substring(l.GetIndex(), l.GetIndex()+l.GetLength()));
        Console.WriteLine(" 2 | "); // TODO: proper identing
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("> Advice: ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("{0}", GetErrString(l.GetErr()));
        Console.ResetColor();
        PrintStage(Stage.LEXER);
    }

    public static void PrintStage(Stage s)
    {
        Utils.Log(ConsoleColor.Magenta, "[STAGE]: ", GetStageString(s));
    }
}