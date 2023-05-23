namespace A7.Frontend;

using A7.Utils;

public struct ValNode {

}


public struct Import
{
    public int id_token_index { get; }
    public int str_token_index { get; }

    public Import(int id_index, int str_index)
    {
        id_token_index = id_index;
        str_token_index = str_index;
    }
}

public struct Function
{
    public FunctionType type { get; }

    public Function(FunctionType ftype)
    {
        type = ftype;
    }
}

public struct VariableDef
{
    int id_token_index { get; }
    TypeIndex type { get; }
    Optional<ValNode> value;

    public VariableDef(int id_token_index, TypeIndex type)
    {
        this.id_token_index = id_token_index;
        this.type = type;
    }
}

public struct Record
{
    int id_token_index { get; }
    List<VariableDef> children { get; }

    public Record(int id_token_index, List<VariableDef> children)
    {
        this.id_token_index = id_token_index;
        this.children = children;
    }
}

public struct Variant
{
    int id_token_index { get; }
    List<VariableDef> children { get; }

    public Variant(int id_token_index, List<VariableDef> children)
    {
        this.id_token_index = id_token_index;
        this.children = children;
    }

    public void AddVariant(VariableDef var_def)
    {
        this.children.Add(var_def);
    }
}

public struct Enum
{
    int id_token_index { get; set; }
    List<Token> children_id { get; set; }

    public Enum(int id_token_index, List<Token> children_id) {
        this.id_token_index = id_token_index;
        this.children_id = children_id;
    }
}

public struct Ast
{
    public Token[] tokens { get; }
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

    public void AddFunction(Function fn)
    {
        functions.Add(fn);
    }

    public void AddRecord() {}
}


public class Parser
{
    private Token[] m_tokens { get; }
    private int m_index;
    private int saved_index;

    public Ast m_ast { get; private set; }
    public string filename { get; }
    public string file { get; }

    public ParserErr m_error { get; set; } = ParserErr.UNKNOWN;

    public Parser(ref Lexer _lexer)
    {
        this.filename = _lexer.filename;
        this.file = _lexer.m_file;
        this.m_tokens = _lexer.GetTokens();
        this.m_ast = new Ast(this.m_tokens);
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
            case TknType.Identifier: s = ParseGlobalAfterIdentifier(); break;
            case TknType.EOT: return Status.Done;
            default: break;
        }

        return s;
    }


    private Status ParseGlobalAfterIdentifier()
    {
        Advance();
        if (ExpectAndConsume(TknType.Colon) == Status.Failure) return Status.Failure;
        if (ExpectAndConsume(TknType.Colon) == Status.Success)
        { return ParseGlobalDefinition(); }

        ParseType();
        Utilities.Todo("Handle Global Mutables & Unwanted Tokens");
        return Status.Failure;
    }

    private Status ParseGlobalDefinition()
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
        Utilities.Todo("Handle Global Constants");
        return Status.Failure;
    }

    private Optional<TypeIndex> ParseType()
    {
        Utilities.Todo("Parse Types");
        return new Optional<TypeIndex>();
    }

    private Status ParseRecords()
    {
        Advance(); // skip 'record'
        Utilities.Todo("Parse Records");
        return Status.Failure;
    }

    private Status ParseVariants()
    {
        Advance(); // skip 'variant'
        Utilities.Todo("Parse Variants");
        return Status.Failure;
    }

    private Status ParseEnums()
    {
        Advance(); // skip 'enum'
        Utilities.Todo("Parse Enums");
        return Status.Failure;
    }

    private Status ParseFunctions()
    {
        /*
         * id :: fn() { ... }
         * id :: fn() return_type { ... }
         * id :: fn(arg1: type1, ...) { ... }
         * id :: fn(arg1: type1, ...) return_type { ... }
         */
        Advance(); // skip 'fn'
        Utilities.Todo("Parse Functions");
        return Status.Failure;
    }

    private Status ParseImports()
    {
        // io :: import "string_path"
        // ^^-^^--- already advanced

        Advance(); // skip 'import'

        // Expect a string else fail
        if (ExpectAndConsume(TknType.StringLiteral, ParserErr.IMPORT_EXPECT_STRING)
            == Status.Failure)
            return Status.Failure;

        // Add identifier index as (index-2), and string index as current index
        m_ast.AddImport(m_index - 2, m_index);
        // pretty much done, only requires a terminator

        // return status of finding a statement terminator
        Status res = ExpectAndConsume(TknType.Terminator, ParserErr.IMPORT_EXPECT_TERMINATOR);
        return res;
    }

    private Status ExpectAndConsume(TknType type, ParserErr err = ParserErr.UNKNOWN)
    {
        // NOTE: if next token's type matched the token type then Advance(), else report failure
        Token c = CurrentTkn();
        bool eql = c.type == type;
        if (eql) { Advance(); }
        else
        {
            m_error = err;
#if DEBUG
            Utilities.LogDebug(c.ToString());
#endif
        }
        return eql ? Status.Success : Status.Failure;
    }


    private void SaveState() { saved_index = m_index; }
    private void RestoreState() { m_index = saved_index; }
    private Token CurrentTkn() { return m_tokens[m_index]; }
    private Token NextTkn() { return m_tokens[(m_index + 1)]; }
    private Token PrevTkn() { return m_tokens[(m_index - 1)]; }
    private void Advance() { ++m_index; }


    // other "useless" methods
    public Token GetCurrentToken() { return CurrentTkn(); }

}
