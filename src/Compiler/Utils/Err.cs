namespace A7.Utils;

using System;
using System.Text;
using A7.Frontend;

public enum ErrKind
{
    // LEXER
    UNKNOWN,
    STR_NOT_CLOSED,
    STR_NOT_CLOSED_SINGLE_LINE,
    NUM_TOO_LONG,
    STR_TOO_LONG,
    ID_TOO_LONG,
    BUILTIN_ID_TOO_LONG,
    NOT_VALID_ESC_CHAR,
    INVALID_CHAR_LITERAL,
    // PARSER
}

public enum Stage
{
    READ_FILE,
    LEXER,
    PARSER,
    TYPE_CHECK,
    CODE_GEN,
}


public class Err
{

    private static string GetErrString(ErrKind kind)
    {
        switch (kind)
        {
            case ErrKind.UNKNOWN: return "Unknown";
            case ErrKind.STR_NOT_CLOSED: return "String not closed";
            case ErrKind.STR_NOT_CLOSED_SINGLE_LINE: return "String not closed, String literals are not allowed on multi-lines";
            case ErrKind.NUM_TOO_LONG: return "Number literal is too long";
            case ErrKind.STR_TOO_LONG: return "String literal is too long";
            case ErrKind.ID_TOO_LONG: return "Identifier literal is too long";
            case ErrKind.BUILTIN_ID_TOO_LONG: return "Builtin Identifier is too long";
            case ErrKind.NOT_VALID_ESC_CHAR: return "Not a valid escape char";
            case ErrKind.INVALID_CHAR_LITERAL: return "Char literal is too long";
            default: break;
        }
        Utilities.Todo("Add an Error string for " + kind.ToString());
        return "";
    }

    private static string GetStageString(Stage stage)
    {
        switch (stage)
        {
            case Stage.READ_FILE: return "READ FILE";
            case Stage.LEXER: return "LEXER";
            case Stage.PARSER: return "PARSER";
            case Stage.TYPE_CHECK: return "TYPE CHECK";
            case Stage.CODE_GEN: return "CODE GEN";
            default: break;
        }
        Utilities.Todo("Add an Stage string for " + stage.ToString());
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
#if DEBUG
        Utilities.LogInfo("lex.idx:" + l.GetIndex() + ", len:" + l.GetLength());
#endif
        string codeInline = l.m_file.Substring(l.GetIndex(),
             l.GetLength());
        Console.WriteLine(" {0} | {1}", l.GetLine(), codeInline);
        Console.ResetColor();
        Console.Write("   | ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("{0}", GetArrows(l.GetLength()));
        Console.Write("> Advice: ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("{0}", GetErrString(l.GetErr()));
        Console.ResetColor();
        PrintStage(Stage.LEXER);
    }

    private static string GetArrows(int length)
    {
        int resLen = length > 50 ? 50 : length;
        string result = new StringBuilder("^".Length * resLen)
                            .Insert(0, "^", resLen)
                            .ToString();
        return result;
    }

    private static void PrintStage(Stage s)
    {
        Utilities.Log(ConsoleColor.Magenta, "[STAGE]: ", GetStageString(s));
    }

    public static void ParserErrMsg(Parser parser)
    {
        Utilities.Todo("Implement parser error msgs");
    }
}
