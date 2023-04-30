
namespace A7
{
    public static class TokenMethods
    {
        public static string GetKeywordStr(TknType type)
        {
            switch (type)
            {
                case TknType.IfKeyword: return "if";
                case TknType.ElseKeyword: return "else";
                case TknType.AndKeyword: return "and";
                case TknType.ForKeyword: return "for";
                case TknType.BrkKeyword: return "break";
                case TknType.IntKeyword: return "int";
                case TknType.RefKeyword: return "ref";
                case TknType.BoolKeyword: return "bool";
                case TknType.CharKeyword: return "char";
                case TknType.EnumKeyword: return "enum";
                case TknType.FallKeyword: return "fall";
                case TknType.UIntKeyword: return "uint";
                case TknType.FloatKeyword: return "float";
                case TknType.MatchKeyword: return "match";
                case TknType.DeleteKeyword: return "delete";
                case TknType.ImportKeyword: return "import";
                case TknType.PublicKeyword: return "pub";
                case TknType.RecordKeyword: return "record";
                case TknType.ReturnKeyword: return "ret";
                case TknType.ForEachKeyword: return "foreach";
                case TknType.VariantKeyword: return "variant";
                case TknType.FunctionKeyword: return "fn";
            }
            Utils.UNREACHABLE();
            return "";
        }
    }



    public struct Token
    {
        public int index, length, line;
        public TknType type;


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
        BuiltinFunc, // @id
        As, // 'as'
        DeleteKeyword, // 'del'
        NewKeyword, // 'new'
        Equal, // =
        IntegerLiteral, // refers to 10 digits ints
        IntKeyword, // 'int'
        UIntKeyword, // 'uint'
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
        PublicKeyword, // 'pub'
        MatchKeyword, // 'match' basically switch
        EnumKeyword, // 'enum'
        EqualEqual, // ==
        BrkKeyword, // 'break' // only loops
        FallKeyword, // 'fall' // only switch statements
        AddEqual, // +=
        SubEqual, // -=
        MultEqual, // *=
        DivEqual, // /=
        RecordKeyword, // 'record' basically struct
        VariantKeyword, // 'variant' basically tagged union
        RefKeyword, // 'ref' // TODO later
        NilLiteral, // `nil` basically null
        EOT, // END OF TOKENS (last token type in list)

        // WARN: Never use this as a token type
        COUNT, // Used for counting the number of members of the enum
    }
}


