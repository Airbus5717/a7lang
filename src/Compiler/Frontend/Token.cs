
namespace A7.Frontend;


public struct Token
{
    public int index { get; }
    public int length { get; }
    public int line { get; }
    public TknType type { get; }


    public Token(int index, int length, int line, TknType type)
    {
        this.index = index;
        this.length = length;
        this.line = line;
        this.type = type;
    }

    public override string ToString()
    {
        return "Token{index: " + index + ", length: " + length + ", line: " + line + "}";
    }

    public bool Equals(Token token)
    {
        return index == token.index &&
              length == token.length &&
              line == token.line &&
              type == token.type;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(index, length, line, type);
    }
}

public enum TknType : byte
{
    Identifier, // ids
    BuiltinId, // @id
    AsKeyword, // 'as'
    DeleteKeyword, // 'del'
    NewKeyword, // 'new'
    Equal, // =
    IntegerLiteral, // refers to 10 digits ints
    IntKeyword, // 'int'
    UIntKeyword, // 'uint'
    FloatLiteral, // refer to floats
    FltKeyword, // 'flt'
    StringLiteral, // refer to strings
    CharLiteral, // refers to chars
    CharKeyword, // 'char'
    TrueLiteral, // 'true'
    FalseLiteral, // 'false'
    BoolKeyword, // 'bool'
    Terminator, // ; | '\n'
    Colon, // :
    FnKeyword, // 'fn'
    PlusOperator, // +
    MinusOperator, // -
    MultOperator, // *
    DivOperator, // /
    OpenParen, // (
    CloseParen, // )
    OpenCurly, // {
    CloseCurly, // }
    OpenSQRBrackets, // [
    CloseSQRBrackets, // ]
    RetKeyword, // 'ret'
    ImportKeyword, // 'import'
    IfKeyword, // 'if'
    ElseKeyword, // 'else'
    ForKeyword, // 'for'
    ForEachKeyword, // 'foreach'
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
    PubKeyword, // 'pub'
    MatchKeyword, // 'match' basically switch
    EnumKeyword, // 'enum'
    EqualEqual, // ==
    BreakKeyword, // 'break' // only loops
    FallKeyword, // 'fall' // only switch statements
    AddEqual, // +=
    SubEqual, // -=
    MultEqual, // *=
    DivEqual, // /=
    RecordKeyword, // 'record' basically struct
    DeferKeyword, // 'defer' cleanup at end of scope
    VariantKeyword, // 'variant' basically tagged union
    RefKeyword, // 'ref' // TODO later
    NilLiteral, // `nil` basically null
    EOT, // END OF TOKENS (last token type in list)

    // WARN: Never use this as a token type
    COUNT, // Used for counting the number of members of the enum
}


