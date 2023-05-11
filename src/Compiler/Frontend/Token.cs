namespace A7.Frontend;

using A7.Utils;

public static class TokenMethods
{
    public static string GetKeywordStr(TknType type)
    {
        switch (type)
        {
            // NOTE: 2 chars
            case TknType.IfKeyword: return "if";
            case TknType.FnKeyword: return "fn";
            case TknType.AsKeyword: return "as";
            case TknType.OrKeyword: return "or";
            // NOTE: 3 chars
            case TknType.AndKeyword: return "and";
            case TknType.ForKeyword: return "for";
            case TknType.RefKeyword: return "ref";
            case TknType.RetKeyword: return "ret";
            case TknType.PubKeyword: return "pub";
            case TknType.NewKeyword: return "new";
            case TknType.NilLiteral: return "nil";
            case TknType.IntKeyword: return "int";
            case TknType.FltKeyword: return "flt";
            // NOTE: 4 chars
            case TknType.ElseKeyword: return "else";
            case TknType.BoolKeyword: return "bool";
            case TknType.CharKeyword: return "char";
            case TknType.EnumKeyword: return "enum";
            case TknType.FallKeyword: return "fall";
            case TknType.UIntKeyword: return "uint";
            case TknType.TrueLiteral: return "true";
            // NOTE: 5 chars
            case TknType.BreakKeyword: return "break";
            case TknType.MatchKeyword: return "match";
            case TknType.DeferKeyword: return "defer";
            case TknType.FalseLiteral: return "false";
            // NOTE: 6 chars
            case TknType.DeleteKeyword: return "delete";
            case TknType.ImportKeyword: return "import";
            case TknType.RecordKeyword: return "record";
            // NOTE: 7 chars
            case TknType.ForEachKeyword: return "foreach";
            case TknType.VariantKeyword: return "variant";
        }
        Utilities.UNREACHABLE();
        return "";
    }
}


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


