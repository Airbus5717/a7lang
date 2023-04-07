
namespace A7
{
    public struct Token
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

    public enum TknType : byte
    {
        Identifier, // ids
        BuiltinFunc, // @id
        As, // 'as'
        DeleteKeyword, // 'del'
        NewKeyword, // 'new'
        Equal, // =
        IntegerLiteral, // refers to 10 digits ints
        IntKeyword, // 'int'
        UintKeyword, // 'uint'
        FloatLiteral, // refer to floats
        FloatKeyword, // 'flt'
        StringLiteral, // refer to strings
        CharLiteral, // refers to chars
        CharKeyword, // 'char'
        TrueLiteral, // 'true'
        FalseLiteral, // 'false'
        BoolKeyword, // 'bool'
        SemiColon, // ;
        Colon, // :
        FunctionKeyword, // 'fn'
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
        ReturnKeyword, // 'ret'
        LoadKeyword, // 'load' basically import
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
        PublicKeyword, // 'pub'
        MatchKeyword, // 'match' basically switch
        EnumKeyword, // 'enum'
        EqualEqual, // ==
        BrkKeyword, // 'break'
        FallKeyword, // 'fall'
        AddEqual, // +=
        SubEqual, // -=
        MultEqual, // *=
        RecordKeyword, // 'record' basically struct
        DivEqual, // /=
        RefKeyword, // 'ref' // TODO later
        NilLiteral, // `nil` basically null
        EOT, // END OF TOKENS (last token type in list)


        // WARN: Never use this as a token type
        COUNT, // Used for counting the number of members of the enum
    }
}
