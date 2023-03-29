

struct CompileOptions
{
    public string path { get; set; } = "";
    // TODO: more options
    public CompileOptions(string path) { this.path = path; }
}

class Compiler
{
    public static Status compile(CompileOptions opts)
    {
        /* 
            Compiling Stages 
            1. Read File
            2. Lex File
            3. Parse Tokens
            4. Type Check
            5. Optimize (optional)
            6. Intermediate representation
            7. Backend Code generation
        */

        // STAGE: Read File
        var myFile = Utils.ReadFile(opts.path);
        if (myFile == null)
            return Status.Failure;



        // STAGE: Lex File
        // TODO: STAGE: Parse Tokens
        // TODO: STAGE: Type Check
        // TODO: STAGE: Optimize
        // TODO: STAGE: Intermediate representation
        // TODO: STAGE: Backend Code Generation


        return Status.Success;
    }
}