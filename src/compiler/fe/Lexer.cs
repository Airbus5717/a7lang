
class Lexer
{
    List<Token> m_tokens;
    uint m_index, m_length, m_line;
    readonly string m_file;

    // save state variables
    uint save_index, save_length, save_line;

    Lexer(ref string file)
    {
        this.m_file = file;
        this.m_tokens = new List<Token>();
        this.m_line = 1;
    }

    // NOTE(5717): Lexer starting point
    Status Lex()
    {
        Status s = Status.Failure;

        while (true)
        {
            s = LexDirector();
            if (s == Status.Success) continue;
            break;
        }

        if (s == Status.Failure)
        {
            RestoreState();
            Utils.TODO("implement Error message in lexer");
        }
        return s;
    }

    Status LexDirector()
    {
        char c = CurrentChar(), p = PeekChar();

        m_length = 1; // reset length

        SkipWhitespace();

        if (Char.IsAsciiDigit(c))
            return LexNumeric();

        if (Char.IsAsciiLetter(c) || c == '_')
            return LexIdentifier();

        if (c == '"')
            return LexString();

        if (c == '\'')
            return LexChar();

        switch (c)
        {
            case ';': return AddToken(TknType.SemiColon);
            case '{': return AddToken(TknType.OpenCurly);
            case '}': return AddToken(TknType.CloseCurly);
            case '(': return AddToken(TknType.OpenParen);
            case ')': return AddToken(TknType.CloseParen);
            case '[': return AddToken(TknType.OpenSQRBrackets);
            case ']': return AddToken(TknType.CloseSQRBrackets);
            case ',': return AddToken(TknType.Comma);
            case ':': return AddToken(TknType.Colon);
            case '&': return AddToken(TknType.BitwiseAnd);
            case '|': return AddToken(TknType.BitwiseOr);
            case '^': return AddToken(TknType.BitwiseXor);
            case '.':
                {
                    if (p == '.')
                    {
                        // '..'
                        m_length++;
                        return AddToken(TknType.To);
                    }
                    // '.'
                    return AddToken(TknType.Dot);
                }
            case '>':
                {
                    if (p == '=')
                    {
                        // '>='
                        m_length++;
                        return AddToken(TknType.GreaterEql);
                    }
                    else if (p == '>')
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
                    if (p == '=')
                    {
                        // '<='
                        m_length++;
                        return AddToken(TknType.LessEql);
                    }
                    else if (p == '<')
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
                    if (p == '=')
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
                    if (p == '=')
                    {
                        // '+='
                        m_length++;
                        return AddToken(TknType.AddEqual);
                    }
                    // '+'
                    return AddToken(TknType.Plus);
                }
            case '-':
                {
                    if (p == '=')
                    {
                        // '-='
                        m_length++;
                        return AddToken(TknType.SubEqual);
                    }
                    // '-'
                    return AddToken(TknType.Minus);
                }
            case '*':
                {
                    if (p == '=')
                    {
                        // '*='
                        m_length++;
                        return AddToken(TknType.MultEqual);
                    }
                    // '*'
                    return AddToken(TknType.Mult);
                }
            case '/':
                {
                    if (p == '=')
                    {
                        // '/='
                        m_length++;
                        return AddToken(TknType.DivEqual);
                    }
                    else if (p == '/')
                    {
                        // '// single line comments'
                        while (IsNotEOF() && CurrentChar() != '\n')
                            Advance();
                        return Status.Success;
                    }
                    // TODO: Implement Multiline comments
                    // '/'
                    return AddToken(TknType.Div);
                }
            case '!':
                {
                    if (p == '=')
                    {
                        // '!='
                        m_length++;
                        return AddToken(TknType.NotEqual);
                    }
                    // '!'
                    return AddToken(TknType.Not);
                }
            case char.MinValue: // '\0'
                {
                    AddToken(TknType.EOT);
                    AddToken(TknType.EOT);
                    AddToken(TknType.EOT);
                    return Status.Done;
                }
            default:
                break;
        }
        return Status.Failure;
    }

    Status LexNumeric()
    {
        char c = CurrentChar(), p = PeekChar();

        if (c == '0')
        {
            switch (p)
            {
                case 'x': return LexHexIntLiteral();
                case 'b': return LexBinaryIntLiteral();
                default: break;
            }
        }

        bool reached_dot = false;
        while (Char.IsAsciiDigit(CurrentChar()) || CurrentChar() == '.')
        {
            AdvanceWithLength();
            if (CurrentChar() == '.')
            {
                if (reached_dot) break;
                reached_dot = true;
            }
        }

        if (m_length > 0x80)
        {
            Utils.TODO("Handle lexing large number literals");
            return Status.Failure;
        }

        return AddToken(reached_dot ? TknType.FloatLiteral : TknType.IntegerLiteral);
    }

    Status LexHexIntLiteral()
    {
        Utils.TODO("Lex Hexadecimal integer literals");
        return Status.Failure;
    }

    Status LexBinaryIntLiteral()
    {
        Utils.TODO("Lex binary integer literals");
        return Status.Failure;
    }

    Status LexIdentifier()
    {
        Utils.TODO("Lex identifier literals");
        return Status.Failure;
    }

    Status LexChar()
    {
        Utils.TODO("Lex character literals");
        return Status.Failure;
    }

    Status LexString()
    {
        // Utils.TODO("Lex string literals");
        AdvanceWithLength();
        return Status.Failure;
    }

    void SkipWhitespace()
    {
        while (true)
        {
            char c = CurrentChar();
            if (c == ' ') { Advance(); }
            else if (c == '\n')
            {
                Advance();
                m_line++;
            }
            else { break; }
        }
    }

    void Advance() { ++m_index; }
    void AdvanceWithLength() { ++m_index; ++m_length; }

    void SaveState()
    {
        save_index = m_index;
        save_length = m_length;
        save_line = m_line;
    }

    void RestoreState()
    {
        m_index = save_index;
        m_length = save_length;
        m_line = save_line;
    }


    // NOTE: casted are safe within range due to length check during ReadFile 
    char CurrentChar() { return m_file[(int)m_index]; }
    char PeekChar() { return m_file[(int)m_index + 1]; }
    char PastChar() { return m_file[(int)m_index - 1]; }
    // is not end of file
    bool IsNotEOF() { return m_index < (uint)m_file.Length; }

    Status AddToken(TknType type)
    {
        m_tokens.Add(new Token(m_index, m_length, m_line, type));
        m_index += m_length;
        return Status.Success;
    }
}