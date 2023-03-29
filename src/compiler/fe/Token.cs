

struct Token
{
    public uint index, length, line;
    public TknType type;


    public Token(uint index, uint length, uint line, TknType type)
    {
        this.index = index;
        this.length = length;
        this.line = line;
        this.type = type;
    }
}

enum TknType : byte
{
    Identifier = 0, // ids
    BuiltinFunc, //? @ids
    To, // ..
    // ToEQL, // ..=
    // In, // in
    As, // 'as'
    DeleteKeyword, // 'del'
    Equal, // =
    IntegerLiteral, // refers to 10 digits ints
    IntKeyword, // 'int'
    UintKeyword, // 'uint'
    FloatLiteral, // refer to floats
    FloatKeyword, // 'float'
    StringLiteral, // refer to strings
    CharLiteral, // refers to chars
    CharKeyword, // 'char'
    TrueLiteral, // 'correct'
    FalseLiteral, // 'wrong'
    BoolKeyword, // 'bool'
    SemiColon, // ;
    Colon, // :
    FunctionKeyword, // 'fn'
    Plus, // +
    Minus, // -
    Mult, // *
    Div, // /
    OpenParen, // (
    CloseParen, // )
    OpenCurly, // {
    CloseCurly, // }
    OpenSQRBrackets, // [
    CloseSQRBrackets, // ]
    ReturnKeyword, // 'ret'
    LoadKeyword, // 'load' basically import
    IfKeyword, // 'if'
    ElseKeyword, // 'else'
    ForKeyword, // 'for'
    Greater, // >
    GreaterEql, // >=
    Less, // <
    LessEql, // <=
    Dot, // .
    Not, // "!"
    NotEqual, // "!="
    AndKeyword, // 'and' logical and
    BitwiseAnd, // &
    BitwiseOr, // | 
    BitwiseXor, // ^
    LeftShift, // << 
    RightShift, // >>
    OrKeyword, // 'or' logical or
    Comma, // ,
    PublicKeyword, // 'pub'
    MatchKeyword, // 'match' basically switch
    EnumKeyword, // 'enum'
    EqualEqual, // ==
    EscKeyword, // 'escape' basically break
    AddEqual, // +=
    SubEqual, // -=
    MultEqual, // *=
    LayoutKeyword, // 'layout' basically struct
    DivEqual, // /=
    RefKeyword, // 'ref' // TODO later
    NilLiteral, // `nil` basically null
    EOT, // END OF TOKENS (last token type in list)

    COUNT, // Used for counting the number of members of the enum
}