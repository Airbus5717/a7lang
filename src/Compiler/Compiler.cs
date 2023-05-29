namespace A7;
using A7.Utils;
using A7.Frontend;

using CompileStatus = Pair<A7.Utils.Status, A7.Utils.Stage>;

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

    public static CompileStatus compile(CompileOptions opts)
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
        CompileStatus status = new CompileStatus();
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

#if DEBUG
        // TODO: seperate debug compiling for each stage
        DebugCompile(ref lexer); // only at debug builds
#endif
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


    public static void DebugCompile(ref Lexer l)
    {
        // Console.WriteLine("Tokens count: " + (lexer.m_tokens.Count-5));
        Console.WriteLine("File(name={0}, len={1})\n ", l.filename, l.m_file.LongCount());
        if (l.m_file.LongCount() < 0x1000)
        {
            Utils.Err.PrintStage(Stage.READ_FILE);
            Console.WriteLine("```\n{0}```", l.m_file);
            Utils.Err.PrintStage(Stage.LEXER);
            foreach (Token i in l.GetTokens())
            {
                Console.WriteLine(i.ToString());
                if (i.type == TknType.EOT) break;
            }
            Utils.Err.PrintStage(Stage.PARSER);
            // TODO:
        }
        else
        {
            Console.WriteLine("The file is just too large");
        }
    }
}
