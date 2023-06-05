namespace A7.Frontend;

using A7.Utils;

enum BinaryOP : byte
{
    Add,
    Sub,
    Multiply,
    Divide,

    Shift_Left,
    Shift_Right,

    Bitwise_And,
    Bitwise_Or,
    Bitwise_Xor,

    Logical_Or,
    Logical_And,
    // TODO: Add the rest of operations
}
enum UnaryOP : byte
{
    Plus_Operator,
    Negate_Logical,
    Negate_Sign,
    // TODO: Add the rest of operations
}

public interface ValNode
{
    // TODO:
    //
    string ToString()
    {
        return "ValNode";
    }
}

public struct SingleValueNode : ValNode
{
    Token child_tkn { get; set; }
}

public struct BinaryValueNode : ValNode
{
    ValNode left { get; set; }
    ValNode right { get; set; }
    BinaryOP op { get; set; }
}

public struct UnaryValueNode : ValNode
{
    ValNode child { get; set; }
    UnaryOP op { get; set; }
}


public interface Statement
{

}

public struct IfTrue : Statement
{
    ValNode branch_condition { get; set; }
    CodeBlock true_branch { get; set; }
}

public struct DeclVariable : Statement
{
    bool is_const { get; set; }
    Token id { get; set; }
    TypeIndex type { get; set; }
    ValNode value { get; set; }
}

public struct MutateVariable : Statement
{
    ValNode new_val { get; set; }
    Token id { get; set; }
}

public struct FunctionCallStmt : Statement
{

}

public struct CodeBlock
{
    List<Statement> statements { get; set; }
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
    public CodeBlock block { get; }

    public Function(FunctionType ftype, CodeBlock block)
    {
        type = ftype;
        this.block = block;
    }
}

public struct VariableDef
{
    public int id_token_index { get; }
    public TypeIndex type { get; set; }
    public Optional<ValNode> value;

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

    public Enum(int id_token_index, List<Token> children_id)
    {
        this.id_token_index = id_token_index;
        this.children_id = children_id;
    }
}

public struct Ast
{
    public readonly Token[] tokens;
    public List<Import> imports { get; private set; }
    public List<Function> functions { get; private set; }
    public List<Record> records { get; private set; }
    public List<Enum> enums { get; private set; }
    public List<VariableDef> gl_vars { get; private set; }
    public List<Variant> variants { get; private set; }

    public Ast(ref Token[] tkns)
    {
        this.tokens = tkns;
        this.imports = new List<Import>();
        this.functions = new List<Function>();
        this.records = new List<Record>();
        this.enums = new List<Enum>();
        this.variants = new List<Variant>();
    }

    public void AddImport(int id_token_index, int str_token_index)
    {
        imports.Add(new Import(id_token_index, str_token_index));
    }

    public void AddFunction(FunctionType ftype, CodeBlock block)
    {
        functions.Add(new Function(ftype, block));
    }

    public void AddRecord(int id_token_index, List<VariableDef> children)
    {
        records.Add(new Record(id_token_index, children));
    }

    public void AddEnum(int id_token_index, List<Token> children_id)
    {
        enums.Add(new Enum(id_token_index, children_id));
    }

    public void AddGlobalVariable(int id_token_index, TypeIndex type)
    {
        gl_vars.Add(new VariableDef(id_token_index, type));
    }

    public void AddVariant(int id_token_index, List<VariableDef> children)
    {
        variants.Add(new Variant(id_token_index, children));
    }
}


public class Parser
{
    private readonly Token[] m_tokens;
    public readonly string filename;
    public readonly string file;

    public Ast m_ast { get; private set; }

    private int m_index;
    private int saved_index;

    public ParserErr m_error { get; set; } = ParserErr.UNKNOWN;

    public Parser(ref Lexer _lexer)
    {
        // readonly
        this.filename = _lexer.filename;
        this.file = _lexer.m_file;
        this.m_tokens = _lexer.GetTokens();

        this.m_ast = new Ast(ref this.m_tokens);

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
        SkipTerminators();
        // Global Statements
        Status s = Status.Failure;
        SaveState();

        // is? 'pub' (imports should not have pub)
        bool is_public = CurrentTkn().type == TknType.PubKeyword;
        if (is_public) Advance();

        SkipTerminators();

        Token c = CurrentTkn(), n = NextTkn();
        switch (c.type)
        {
            case TknType.Identifier: s = ParseGlobalConstantStatement(is_public); break;
            case TknType.EOT: return Status.Done;
            default: break;
        }

        return s;
    }


    private Status ParseGlobalConstantStatement(bool is_public)
    {
        Advance(); // skips identifier
        SkipTerminators();

        // every global identifier requires a colon after it
        if (ExpectAndConsume(TknType.Colon, ParserErr.EXPECT_GLOBAL_COLON_AFTER_ID) == Status.Failure) return Status.Failure;

        // ::
        //  ^--- 2nd colon
        if (ExpectAndConsume(TknType.Colon) == Status.Success)
            return ParseGlobalDefinition(is_public);

        // :=
        //  ^-- equal
        if (ExpectAndConsume(TknType.Equal) == Status.Success)
            return ParseGlobalExpression();

        // NOTE: else parse type
        // TODO: FIXME: deal with
        // id : type =
        // id : type :
        Utilities.Todo("Deal with parse type for written type");
        Optional<TypeIndex> type = ParseType(TknType.Equal);
        if (ExpectAndConsume(TknType.Equal) == Status.Success)
            return ParseGlobalExpression();

        if (ExpectAndConsume(TknType.Colon) == Status.Success)
            return ParseGlobalDefinition(is_public);

        Utilities.Todo("Handle Unwanted Global Tokens");
        return Status.Failure;
    }

    private Status ParseGlobalExpression()
    {
        // NOTE: For Global Variables
        // not allowed to require runtime related computing
        Utilities.Todo("Handle Global Mutables");
        return Status.Failure;
    }

    private Status ParseGlobalDefinition(bool is_public)
    {
        SkipTerminators();
        if (is_public)
        {
            // NOTE: maybe forgive public imports?
            switch (CurrentTkn().type)
            {
                case TknType.FnKeyword: return ParseFunctions();
                case TknType.RecordKeyword: return ParseRecords();
                case TknType.EnumKeyword: return ParseEnums();
                case TknType.VariantKeyword: return ParseVariants();
                default:
                    Utilities.Todo("Handle Global public tokens");
                    break;
            }
        }
        else
        {
            switch (CurrentTkn().type)
            {
                case TknType.FnKeyword: return ParseFunctions();
                case TknType.RecordKeyword: return ParseRecords();
                case TknType.ImportKeyword: return ParseImports();
                case TknType.EnumKeyword: return ParseEnums();
                case TknType.VariantKeyword: return ParseVariants();
                default:
                    Utilities.Todo("Handle Global public tokens");
                    break;
            }
        }

        Utilities.Todo("Handle Global Constants");
        return Status.Failure;
    }

    private TypeIndex ParseType(TknType t)
    {
        var type = new TypeIndex();
        var current_type = CurrentTkn().type;
        if (current_type == t) return type;
        switch (current_type)
        {
            case TknType.IntKeyword: type.kind = TypeBaseKind.Int; break;
            case TknType.UIntKeyword: type.kind = TypeBaseKind.UInt; break;
            case TknType.FltKeyword: type.kind = TypeBaseKind.Float; break;
            case TknType.CharKeyword: type.kind = TypeBaseKind.Char; break;
            case TknType.BoolKeyword: type.kind = TypeBaseKind.Bool; break;
            case TknType.Identifier: Utilities.Todo("Parse record,variant,enum types & custom types"); break;
            case TknType.OpenSQRBrackets: Utilities.Todo("Parse Array types"); break;
            // case TknType.OpenCurly: break;
            // case TknType.Comma: break;
            // case TknType.OpenParen: break;
            // case TknType.Equal: break;
            // case TknType.CloseCurly: break;
            // case TknType.CloseParen: break;
            // case TknType.CloseSQRBrackets: break;
            // case TknType.Terminator: break;
            default:
#if DEBUG
                Utilities.LogDebug("Parse invalid Type with token type: " + CurrentTkn().type);
#endif
                break;

        }
        return type;
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
        if (ExpectAndConsume(TknType.OpenParen, ParserErr.FN_EXPECT_OPEN_PARENTHESES) == Status.Failure) return Status.Failure;
        List<VariableDef> args = ParseFunctionArguments();
        if (ExpectAndConsume(TknType.CloseParen, ParserErr.FN_EXPECT_CLOSE_PARENTHESES) == Status.Failure) return Status.Failure;
        TypeIndex return_type = ParseType(TknType.OpenCurly); // Parse Type Until a delimiter which is '{'

        if (ExpectAndConsume(TknType.OpenCurly, ParserErr.FN_EXPECT_BODY) == Status.Failure)
        {
            Utilities.Todo("Handle Function definitions without impl");
            return Status.Failure;
        }

        CodeBlock blk = ParseCodeBlock();

        if (ExpectAndConsume(TknType.CloseCurly, ParserErr.FN_EXPECT_END_CLOSE_CURLY) == Status.Failure) return Status.Failure;

        m_ast.AddFunction(new FunctionType(args, return_type), blk);
        return Status.Success;
    }

    private CodeBlock ParseCodeBlock()
    {
        var blk = new CodeBlock();
        Utilities.Todo("Parse Code Blocks");
        while (true)
        {
            // TODO:
            break;
        }
        return blk;
    }

    private List<VariableDef> ParseFunctionArguments()
    {
        var c = CurrentTkn();
        List<VariableDef> list = new List<VariableDef>();
        while (true)
        {
            if (c.type == TknType.CloseParen) break;
            else
            {
                Utilities.Todo("implement Parse function arguments");
            }
        }
        return list;
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

    private void SkipTerminators()
    {
        TknType curr = CurrentTkn().type;
        if (curr == TknType.Terminator) Advance();
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
