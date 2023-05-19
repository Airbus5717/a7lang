namespace A7;
using A7.Utils;
using A7.Frontend;

struct CompileOptions
{
    public string path;

    // TODO: more options
    public CompileOptions(string path)
    {
        this.path = path;
    }
}

public class Pair<T, K>
{
    public T item1 { get; set; }
    public K item2 { get; set; }
}

class Compiler
{

    public static Pair<Status, Stage> compile(CompileOptions opts)
    {
        /*
            Compiling Stages
            1. Read File -> File [DONE]
            2. Lex File -> Tokens []
            3. Parse Tokens -> Ast
            4. Type Check -> Checked Ast
            5. Optimize (optional) -> partially Optimized Ast
            7. Backend Code generation -> output
        */
        Pair<Status, Stage> status = new Pair<Status, Stage>();
        status.item1 = Status.Failure;

        // STAGE: Read File
        status.item2 = Stage.READ_FILE;
        var myFile = Utilities.ReadFile(opts.path);
        if (!myFile.HasValue)
            return status;


        string file = myFile.Value;
        // STAGE: Lex File
        status.item2 = Stage.LEXER;
        var lexer = new Lexer(opts.path, ref file);
        if (lexer.Lex() == Status.Failure)
            return status;

        LogLexer(ref lexer); // only at debug builds
        // STAGE: Parse Tokens
        status.item2 = Stage.PARSER;
        var parser = new Parser(ref lexer);
        if (parser.Parse() == Status.Failure)
            return status;

        // TODO: STAGE: Type Check
        // TODO: STAGE: Optimize
        // TODO: STAGE: Intermediate representation
        // TODO: STAGE: Backend Code Generation

        status.item1 = Status.Done;
        return status;
    }


    public static void LogLexer(ref Lexer l)
    {
#if DEBUG
        // Console.WriteLine("Tokens count: " + (lexer.m_tokens.Count-5));
        if (l.m_file.LongCount() > 0x1000)
        {
            Console.WriteLine("File(len={0})\n ", l.m_file.LongCount());
        }
        else
        {
            Console.WriteLine("File(len={0})\n ", l.m_file.LongCount());
            foreach (Token i in l.GetTokens())
            {
                Console.WriteLine("[Token]: idx: {0}, len: {1}, type: {2}", i.index, i.length, i.type);
                if (i.type == TknType.EOT) break;
            }
        }
#endif
    }
}
