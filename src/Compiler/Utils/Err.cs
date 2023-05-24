namespace A7.Utils;

using System;
using System.Text;
using A7.Frontend;

public enum LexerErr
{
    UNKNOWN,
    STR_NOT_CLOSED,
    STR_NOT_CLOSED_SINGLE_LINE,
    NUM_TOO_LONG,
    STR_TOO_LONG,
    ID_TOO_LONG,
    BUILTIN_ID_TOO_LONG,
    NOT_VALID_ESC_CHAR,
    INVALID_CHAR_LITERAL,
}

public enum ParserErr
{
    UNKNOWN,
    IMPORT_EXPECT_STRING,
    IMPORT_EXPECT_TERMINATOR,
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

    private static string GetLexerErrString(LexerErr kind)
    {
        switch (kind)
        {
            case LexerErr.UNKNOWN: return "Unknown";
            case LexerErr.STR_NOT_CLOSED: return "String not closed";
            case LexerErr.STR_NOT_CLOSED_SINGLE_LINE: return "String not closed, String literals are not allowed on multi-lines";
            case LexerErr.NUM_TOO_LONG: return "Number literal is too long";
            case LexerErr.STR_TOO_LONG: return "String literal is too long";
            case LexerErr.ID_TOO_LONG: return "Identifier literal is too long";
            case LexerErr.BUILTIN_ID_TOO_LONG: return "Builtin Identifier is too long";
            case LexerErr.NOT_VALID_ESC_CHAR: return "Not a valid escape char";
            case LexerErr.INVALID_CHAR_LITERAL: return "Char literal is too long";
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
        string filename = l.filename;
        int line_number = l.GetLine();
        int index = l.GetIndex();
        int length = l.GetLength();
        string code_inline = l.m_file.Substring(index, length);
        string arrows = GetArrows(length);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("> File: {0}:{1}:", filename, line_number);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(" error: ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(l.GetErr());
        Console.ResetColor();
#if DEBUG
        Utilities.LogInfo("lex.idx:" + index + ", len:" + length);
#endif
        Console.WriteLine(" {0} | {1}", line_number, code_inline);
        Console.ResetColor();
        Console.Write("   | ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("{0}", arrows);
        Console.Write("> Advice: ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("{0}", GetLexerErrString(l.GetErr()));
        Console.ResetColor();
        PrintStage(Stage.LEXER);
    }

    private static string GetArrows(int length)
    {
        const int MAX_LENGTH = 50;
        int resLen = length > MAX_LENGTH ? MAX_LENGTH : length;
        string result = new StringBuilder("^".Length * resLen)
                            .Insert(0, "^", resLen)
                            .ToString();
        if (length > MAX_LENGTH) result += "...";
        return result;
    }

    private static string GetSpaces(int length)
    {
        int resLen = length;
        string result = new StringBuilder(" ".Length * resLen)
                            .Insert(0, " ", resLen)
                            .ToString();
        return result;
    }

    public static void PrintStage(Stage s)
    {
        Utilities.Log(ConsoleColor.Magenta, "[STAGE]: ", GetStageString(s));
    }

    public static void ParserErrMsg(Parser parser)
    {
        Token c = parser.GetCurrentToken();
        int line = c.line;
        int index = c.index;
        int length = c.length;

        string code_inline = parser.file.Substring(index, length);
        int indexOfPrevLine = 0;
        for (int i = index; i >= 0; i--)
        {
            indexOfPrevLine = i;
            if (parser.m_ast.tokens[i].line != line) break;
        }
        int spaces = index - indexOfPrevLine;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("> File: {0}:{1}:", parser.filename, line);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(" error: ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(parser.m_error);
        Console.ResetColor();
#if DEBUG
        Utilities.LogInfo("lex.idx:" + index + ", len:" + length);
#endif
        Console.WriteLine(" {0} | {1}", line, code_inline);
        Console.ResetColor();
        Console.Write("   | ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("{0}{1}", GetSpaces(spaces), GetArrows(length));
        Console.Write("> Advice: ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("{0}", GetParserErrString(parser.m_error));
        Console.ResetColor();
        //        PrintStage(Stage.PARSER);
#if DEBUG
        Utilities.PrintStackTrace();
#endif

    }

    private static string GetParserErrString(ParserErr value)
    {
        switch (value)
        {
            case ParserErr.UNKNOWN: return "Unknown";
        }

        Utilities.Todo("implement convert to string for parser errors");
        return "";
    }
}
