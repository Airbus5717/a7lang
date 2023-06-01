namespace A7.Frontend;

using A7.Utils;

/*
    NOTE: Terminator: is a newline or semicolon
*/

/*
TODO List:
    - Multiline comments
*/

public class Lexer
{
    // NOTE: These are not the final length check
    // but these are used just to filter out
    private static int MAX_LENGTH_STRING { get; } = int.MaxValue >> 2;
    private static int MAX_LENGTH_INT_DECIMAL { get; } = 30;
    private static int MAX_LENGTH_INT_BINARY { get; } = 70;
    private static int MAX_LENGTH_INT_HEX { get; } = 20;
    private static int MAX_LENGTH_IDENTIFIER { get; } = 128;

    private List<Token> m_tokens;
    private int m_index, m_length, m_line;
    private LexerErr m_error;

    public string m_file { get; }
    public string filename { get; }

    // save state variables
    // for restoring state in error flow
    private int save_index, save_line;

    private static readonly Dictionary<string, TknType> keyword_map = new Dictionary<string, TknType>{
            { "if", TknType.IfKeyword}, { "fn", TknType.FnKeyword},
            { "as", TknType.AsKeyword}, { "or", TknType.OrKeyword},

            { "and", TknType.AndKeyword}, { "for", TknType.ForKeyword},
            { "ref", TknType.RefKeyword}, { "ret", TknType.RetKeyword},
            { "pub", TknType.PubKeyword}, { "new", TknType.NewKeyword},
            { "nil", TknType.NilLiteral}, { "int", TknType.IntKeyword},
            { "flt", TknType.FltKeyword},

            { "else", TknType.ElseKeyword},
            { "bool", TknType.BoolKeyword}, { "char", TknType.CharKeyword},
            { "enum", TknType.EnumKeyword}, { "fall", TknType.FallKeyword},
            { "uint", TknType.UIntKeyword}, { "true", TknType.TrueLiteral},

            { "break", TknType.BreakKeyword}, { "match", TknType.MatchKeyword},
            { "defer", TknType.DeferKeyword}, { "false", TknType.FalseLiteral},

            { "delete", TknType.DeleteKeyword}, { "import", TknType.ImportKeyword},
            { "record", TknType.RecordKeyword},

            { "foreach", TknType.ForEachKeyword}, { "variant", TknType.VariantKeyword}
        };

    public Lexer(string filename, ref string file)
    {
        this.m_file = file;
        this.filename = filename;
        this.m_tokens = new List<Token>(file.Length / 4);
        this.m_line = 1;
        this.m_index = 0;
        this.m_error = LexerErr.UNKNOWN;
    }

    // NOTE(5717): Lexer starting point
    public Status Lex()
    {
        Status s = Status.Failure;

        while (true)
        {
            s = LexDirector();
            if (s == Status.Success) continue;
            // if status == (Done | Failure) then break
            break;
        }

        if (s == Status.Failure)
        { RestoreState(); Err.LexerErrMsg(this); }
        return s;
    }

    private Status LexDirector()
    {
        char c = CurrentChar(), n = NextChar();
        SaveState();
        SkipWhitespace();

        m_length = 0; // reset length
        if (Char.IsAsciiDigit(c))
            return LexNumeric();

        if (Char.IsAsciiLetter(c) || c == '_')
            return LexIdentifier();

        m_length++; // to simplify the code

        switch (c)
        {
            case '"': return LexString();
            case '\'': return LexChar();
            case '`': return LexMultiLineString();
            case '@': return LexBuiltin();
            /*
             * NOTE(5717): the code below won't increment the index, but rather only the
             * length to remove the need to restore indexes during adding the token
             * to the list during AddToken function call
            */
            case '{': return AddToken(TknType.OpenCurly);
            case '}': return AddToken(TknType.CloseCurly);
            case '(': return AddToken(TknType.OpenParen);
            case ')': return AddToken(TknType.CloseParen);
            case '[': return AddToken(TknType.OpenSQRBrackets);
            case ']': return AddToken(TknType.CloseSQRBrackets);
            case ',': return AddToken(TknType.Comma);
            case '&': return AddToken(TknType.BitwiseAnd);
            case '|': return AddToken(TknType.BitwiseOr);
            case '^': return AddToken(TknType.BitwiseXor);
            case ':':
                {
                    // if (n == ':')
                    // {
                    //     m_length++;
                    //     return AddToken(TknType.DoubleColon);
                    // }
                    return AddToken(TknType.Colon);
                }
            case '.':
                {
                    // if (n == '.')
                    // {
                    //     // '..'
                    //     m_length++;
                    //     return AddToken(TknType.To);
                    // }

                    // '.'
                    return AddToken(TknType.Dot);
                }
            case '>':
                {
                    if (n == '=')
                    {
                        // '>='
                        m_length++;
                        return AddToken(TknType.GreaterEql);
                    }
                    else if (n == '>')
                    {
                        // '>>'
                        m_length++;
                        return AddToken(TknType.RightShift);
                    }
                    // '>'
                    return AddToken(TknType.Greater);
                }
            case '<':
                {
                    if (n == '=')
                    {
                        // '<='
                        m_length++;
                        return AddToken(TknType.LessEql);
                    }
                    else if (n == '<')
                    {
                        // '<<'
                        m_length++;
                        return AddToken(TknType.LeftShift);
                    }
                    // '<'
                    return AddToken(TknType.Less);
                }
            case '=':
                {
                    if (n == '=')
                    {
                        // '=='
                        m_length++;
                        return AddToken(TknType.EqualEqual);
                    }
                    // '='
                    return AddToken(TknType.Equal);
                }
            case '+':
                {
                    if (n == '=')
                    {
                        // '+='
                        m_length++;
                        return AddToken(TknType.AddEqual);
                    }
                    // '+'
                    return AddToken(TknType.PlusOperator);
                }
            case '-':
                {
                    if (n == '=')
                    {
                        // '-='
                        m_length++;
                        return AddToken(TknType.SubEqual);
                    }
                    // '-'
                    return AddToken(TknType.MinusOperator);
                }
            case '*':
                {
                    if (n == '=')
                    {
                        // '*='
                        m_length++;
                        return AddToken(TknType.MultEqual);
                    }
                    // '*'
                    return AddToken(TknType.MultOperator);
                }
            case '/':
                {
                    if (n == '=')
                    {
                        // '/='
                        m_length++;
                        return AddToken(TknType.DivEqual);
                    }
                    else if (n == '/')
                    {
                        // '// single line comments'
                        while (IsNotEOF() && CurrentChar() != '\n')
                            Advance();
                        return Status.Success;
                    }
                    // TODO: Implement Multiline comments
                    // else if (p == '*')
                    // {
                    //     return LexMultiLineComments();
                    // }

                    // '/'
                    return AddToken(TknType.DivOperator);
                }
            case '!':
                {
                    if (n == '=')
                    {
                        // '!='
                        m_length++;
                        return AddToken(TknType.NotEqual);
                    }
                    // '!'
                    return AddToken(TknType.Not);
                }
            case ' ':
            case '\t':
            case '\r':
            case '\n':
                // NOTE: SkipWhitespace() will deal with it, next iteration
                return Status.Success;
            case ';':
                {
                    AddTerminator();
                    return Status.Success;
                }
            case char.MinValue: // '\0'
                {
                    // NOTE(5717): Extra END OF TOKENS as delimiters
                    for (uint i = 0; i < Utilities.NULL_TERMINATORS_COUNT_PASSES; ++i)
                        AddToken(TknType.EOT);
                    return Status.Done;
                }
            default:
                break;
        }
        m_error = LexerErr.UNKNOWN;
        return Status.Failure;
    }

    private Status LexNumeric()
    {

        char c = CurrentChar(), n = NextChar();

        if (c == '0')
        {
            switch (n)
            {
                case 'x': return LexHexIntLiteral();
                case 'b': return LexBinaryIntLiteral();
                // case 'o': return LexOctalIntLiteral();
                default: break;
            }
        }

        bool reached_dot = false;
        AdvanceWithLength();

        // TODO(5717): remove extra period('.') checks
        while (Char.IsAsciiDigit(CurrentChar()) || CurrentChar() == '.' || CurrentChar() == '_')
        {
            AdvanceWithLength();
            if (CurrentChar() == '.')
            {
                if (reached_dot) break;
                reached_dot = true;
            }
        }


        if (m_length > MAX_LENGTH_INT_DECIMAL)
        { m_error = LexerErr.NUM_TOO_LONG; return Status.Failure; }

        RestoreIndex();
        return AddToken(reached_dot ? TknType.FloatLiteral : TknType.IntegerLiteral);
    }

    // 2 small util functions in LexNumeric
    Status LexHexIntLiteral()
    {
        AdvanceWithLength(); // '0'
        AdvanceWithLength(); // 'x'
        while (Char.IsAsciiHexDigit(CurrentChar()) || CurrentChar() == '_')
            AdvanceWithLength();

        if (m_length > MAX_LENGTH_INT_HEX)
        { m_error = LexerErr.NUM_TOO_LONG; return Status.Failure; }
        RestoreIndex();
        return AddToken(TknType.IntegerLiteral);
    }

    Status LexBinaryIntLiteral()
    {
        AdvanceWithLength(); // '0'
        AdvanceWithLength(); // 'b'
        while (IsCurrentABinaryCharOrUnderScore())
            AdvanceWithLength();

        if (m_length > MAX_LENGTH_INT_BINARY)
        { m_error = LexerErr.NUM_TOO_LONG; return Status.Failure; }
        RestoreIndex();
        return AddToken(TknType.IntegerLiteral);
    }



    private Status LexBuiltin()
    {
        Advance(); // skip '@', length is already 1

        while (Char.IsAsciiLetter(CurrentChar()) || CurrentChar() == '_')
        {
            AdvanceWithLength();
        }


        if (m_length > MAX_LENGTH_IDENTIFIER)
        { m_error = LexerErr.BUILTIN_ID_TOO_LONG; return Status.Failure; }

        RestoreIndex();
        return AddToken(TknType.BuiltinId);
    }

    private Status LexIdentifier()
    {
        AdvanceWithLength(); // skip the first char
        while (Char.IsAsciiLetterOrDigit(CurrentChar()) || CurrentChar() == '_')
        {
            AdvanceWithLength();
        }
        // restore the index for comparing
        RestoreIndex();

        string id = m_file.Substring(m_index, m_length);
        // NOTE: get token type for keyword (default to identifier)
        TknType type = keyword_map.GetValueOrDefault(id, TknType.Identifier);

        if (m_length > MAX_LENGTH_IDENTIFIER)
        { m_error = LexerErr.ID_TOO_LONG; return Status.Failure; }

        return AddToken(type);
    }

    private Status LexChar()
    {
        Advance(); // skip ', length is already 1
        if (CurrentChar() != '\\' && NextChar() == '\'')
        {
            AdvanceWithLength();
            AdvanceWithLength();
            RestoreIndex();
            return AddToken(TknType.CharLiteral);
        }
        else if (CurrentChar() == '\\')
        {
            AdvanceWithLength();
            switch (CurrentChar())
            {
                case 'n':
                case 't':
                case 'r':
                case '\\':
                case '\'':
                    { AdvanceWithLength(); break; }
                default:
                    m_error = LexerErr.NOT_VALID_ESC_CHAR;
                    return Status.Failure;
            }
            if (CurrentChar() == '\'')
            {
                AdvanceWithLength();
                RestoreIndex();
                return AddToken(TknType.CharLiteral);
            }
        }
        m_error = LexerErr.INVALID_CHAR_LITERAL;
        return Status.Failure;
    }

    private Status LexString()
    {
        Advance(); // skip '"', length is already 1
        while (true)
        {
            char c = CurrentChar();
            switch (c)
            {
                case '"':
                    if (PrevChar() == '\\')
                    {
                        AdvanceWithLength();
                        continue;
                    }
                    break;
                case char.MinValue:
                    {
                        m_error = LexerErr.STR_NOT_CLOSED;
                        return Status.Failure;
                    }
                case '\n':
                    {
                        m_error = LexerErr.STR_NOT_CLOSED_SINGLE_LINE;
                        return Status.Failure;
                    }
                default:
                    AdvanceWithLength();
                    continue;
            }
            break;
        }
        AdvanceWithLength(); // '"'

        if (m_length > MAX_LENGTH_STRING)
        { m_error = LexerErr.STR_TOO_LONG; return Status.Failure; }

        RestoreIndex();
        return AddToken(TknType.StringLiteral);
    }

    private Status LexMultiLineString()
    {
        Advance(); // skip `, length is already 1
        while (true)
        {
            char c = CurrentChar();
            switch (c)
            {
                case '`':
                    if (PrevChar() == '\\')
                    {
                        AdvanceWithLength();
                        continue;
                    }
                    break;
                case char.MinValue:
                    {
                        m_error = LexerErr.STR_NOT_CLOSED;
                        return Status.Failure;
                    }
                default:
                    AdvanceWithLength();
                    continue;
            }
            break;
        }
        AdvanceWithLength(); // '"'

        if (m_length > MAX_LENGTH_STRING)
        { m_error = LexerErr.STR_TOO_LONG; return Status.Failure; }

        RestoreIndex();
        return AddToken(TknType.StringLiteral);
    }


    // Status LexMultiLineComments()
    // { This Code contains bugs
    //     Utilities.Todo("implement Lex Multi line comments; Nested comments too");
    //     Advance(); // '/'
    //     Advance(); // '*'
    //     uint deepness = 1;
    //     char c = CurrentChar(), n = PeekChar();
    //     while (true)
    //     {
    //         if (c == '/' && n == '*') { deepness++; Advance(); Advance(); }
    //         if (c == '*' && n == '/') { deepness--; Advance(); Advance(); }
    //         // update
    //         Advance();
    //         c = n;
    //         n = PeekChar();
    //         if (deepness == 0) break;
    //         if (n == char.MinValue) break;
    //     }
    //     Advance();
    //     return deepness == 0 ? Status.Success : Status.Failure;
    // }

    private void SkipWhitespace()
    {
        m_length = 1;
        while (true)
        {
            char c = CurrentChar();
            if (c == ' ' || c == '\t') { Advance(); }
            else if (c == '\n')
            {
                AddTerminator();
                m_line++;
            }
            else { break; }
        }
    }

    private void AddTerminator()
    {
        // NOTE: No need to add duplicate terminators
        // if the previous token is already a terminator
        if (m_tokens.LongCount() > 0 && m_tokens.Last().type != TknType.Terminator)
            AddToken(TknType.Terminator);
        else
            Advance();
    }

    private void Advance() { ++m_index; }
    private void AdvanceWithLength() { ++m_index; ++m_length; }

    //* FOR ERROR HANDLING
    private void SaveState()
    {
        save_index = m_index;
        save_line = m_line;
    }

    //* FOR ERROR HANDLING
    private void RestoreState()
    {
        m_index = save_index;
        m_line = save_line;
    }

    private void RestoreIndex()
    {
        m_index -= m_length;
    }

    private char CurrentChar() { return m_file[m_index]; }
    private char NextChar() { return m_file[(m_index + 1)]; }
    private char PrevChar() { return m_file[(m_index - 1)]; }

    private bool IsCurrentABinaryCharOrUnderScore()
    {
        char c = CurrentChar();
        return (c == '0' || c == '1' || c == '_');
    }

    // is not end of file
    private bool IsNotEOF()
    {
        bool res = (CurrentChar() != char.MinValue) && (m_index < m_file.Length);
        return res;
    }

    private Status AddToken(TknType type)
    {
        m_tokens.Add(new Token(m_index, m_length, m_line, type));
        m_index += (m_length); // ADD one to go to the next
        return Status.Success;
    }


    // Other "useless" methods
    public Token[] GetTokens() { return m_tokens.ToArray<Token>(); }
    public int GetIndex() { return m_index; }
    public int GetLength() { return m_length; }
    public int GetLine() { return m_line; }
    public LexerErr GetErr() { return m_error; }
}
