
namespace A7
{
    struct CompileOptions
    {
        public string path = "";

        // TODO: more options
        public CompileOptions(string path)
        {
            this.path = path;
        }
    }

    class Compiler
    {
        public static Status compile(CompileOptions opts)
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

            // STAGE: Read File
            var myFile = Utils.ReadFile(opts.path);
            if (myFile.m_res == Res.Err)
                return Status.Failure;



            // STAGE: Lex File
            var lexer = new Lexer(opts.path, ref myFile.m_item);
            if (lexer.Lex() == Status.Failure)
                return Status.Failure;

            // Console.WriteLine("Tokens count: " + (lexer.m_tokens.Count-5));

            Console.WriteLine("File(len={0}):\n {1}", myFile.m_item.Count(), myFile.m_item);
            foreach (var i in lexer.GetTokens())
            {
                if (i.type != TknType.EOT)
                    Console.WriteLine("[Token]: idx: {0}, len: {1}, type: {2}", i.index, i.length, i.type);
            }

            // TODO: STAGE: Parse Tokens
            // TODO: STAGE: Type Check
            // TODO: STAGE: Optimize
            // TODO: STAGE: Intermediate representation
            // TODO: STAGE: Backend Code Generation


            return Status.Success;
        }
    }
}
