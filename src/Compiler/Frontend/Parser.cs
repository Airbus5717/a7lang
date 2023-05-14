namespace A7.Frontend;

using A7.Utils;

public struct Import
{
    public int id_token_index { get; private set; }
    public int str_token_index { get; private set; }

    public Import(int id_index, int str_index)
    {
        id_token_index = id_index;
        str_token_index = str_index;
    }
}

public struct Function
{
    public FunctionType type { get; private set; }

    public Function(FunctionType ftype)
    {
        type = ftype;
    }
}

public struct VariableDef
{
    int id_token_index { get; set; }

}

public struct Record
{
    int id_token_index { get; set; }
    VariableDef[] children { get; set; }

}

public struct Enum
{
    int id_token_index { get; set; }
    Token[] ids { get; set; }
}

public struct Ast
{
    public Token[] tokens { get; private set; }
    public List<Import> imports { get; private set; }
    public List<Function> functions { get; private set; }
    public List<Record> records { get; private set; }
    public List<Enum> enums { get; private set; }

    public Ast(Token[] tkns)
    {
        this.tokens = tkns;
        this.imports = new List<Import>();
        this.functions = new List<Function>();
        this.records = new List<Record>();
        this.enums = new List<Enum>();
    }

    public void AddImport(int id_index, int str_index)
    {
        imports.Add(new Import(id_index, str_index));
    }
}







public class Parser
{
    private Token[] m_tokens { get; }
    private int m_index;
    private int saved_index;

    public Ast ast { get; private set; }
    public string filename { get; }
    public string file { get; }

    public ParserErr m_error { get; }

    public Parser(ref Lexer _lexer)
    {
        this.filename = _lexer.filename;
        this.file = _lexer.m_file;
        this.m_tokens = _lexer.GetTokens();
        this.ast = new Ast(_lexer.GetTokens());
        this.m_index = 0;
        this.saved_index = 0;
        this.m_error = ParserErr.UNKNOWN;
    }


    public Status Parse()
    {
        Status s = Status.Failure;

        while (true)
        {
            s = ParseDirector();
            if (s == Status.Success) continue;
            // if status == (Done | Failure) then break
            break;
        }

        if (s == Status.Failure)
        {
            // RestoreState();
            Err.ParserErrMsg(this);
        }
        return s;
    }

    private Status ParseDirector()
    {
        // Global Statements
        Status s = Status.Failure;
        SaveState();

        // is? 'pub' (imports should not have pub)
        bool is_public = CurrentTkn().type == TknType.PubKeyword;
        if (is_public) Advance();


        Token c = CurrentTkn(), n = NextTkn();
        switch (c.type)
        {
            case TknType.Identifier: s = ParseAfterIdentifier(); break;
            case TknType.EOT: { return Status.Done; }
            default: break;
        }

        return s;
    }


    private Status ParseAfterIdentifier()
    {
        Advance();
        if (ExpectAndConsume(TknType.Colon) == Status.Failure) return Status.Failure;
        if (ExpectAndConsume(TknType.Colon) == Status.Success)
        {
            return ParseDefinition();
        }



        ParseType();
        Utilities.Todo("Handle Global Mutables & Unwanted Tokens");
        return Status.Failure;
    }

    private Status ParseDefinition()
    {
        switch (CurrentTkn().type)
        {
            case TknType.FnKeyword: return ParseFunctions();
            case TknType.ImportKeyword: return ParseImports();
            case TknType.RecordKeyword: return ParseRecords();
            case TknType.EnumKeyword: return ParseEnums();
            case TknType.VariantKeyword: return ParseVariants();

            default:
                break;
        }
        Utilities.Todo("Handle Globals");
        return Status.Failure;
    }

    private Optional<TypeIndex> ParseType()
    {
        Utilities.Todo("Parse Types");
        return new Optional<TypeIndex>();
    }

    private Status ParseRecords()
    {
        Utilities.Todo("Parse Records");
        return Status.Failure;
    }

    private Status ParseVariants()
    {
        Utilities.Todo("Parse Variants");
        return Status.Failure;
    }

    private Status ParseEnums()
    {
        Utilities.Todo("Parse Enums");
        return Status.Failure;
    }

    private Status ParseFunctions()
    {
        Utilities.Todo("Parse Functions");
        return Status.Failure;
    }

    private Status ParseImports()
    {
        Advance(); // 'import'
        if (ExpectAndConsume(TknType.StringLiteral) == Status.Failure) return Status.Failure;
        ast.AddImport(m_index - 2, m_index);
        var res = ExpectAndConsume(TknType.Terminator);
        return res;
    }

    private Status ExpectAndConsume(TknType type)
    {
        Token c = CurrentTkn();
        bool res = c.type == type;
        if (res) { Advance(); }
#if DEBUG
        else
        {
            Utilities.LogDebug(c.ToString());
        }
#endif
        return res ? Status.Success : Status.Failure;
    }


    private void SaveState()
    {
        saved_index = m_index;
    }

    private void RestoreState()
    {
        m_index = saved_index;
    }

    public Token CurrentTkn()
    {
        return m_tokens[m_index];
    }

    private Token NextTkn()
    {
        return m_tokens[(m_index + 1)];
    }

    private Token PrevTkn()
    {
        return m_tokens[(m_index - 1)];
    }

    private void Advance() { ++m_index; }




}
