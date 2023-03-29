
class Lexer
{
    List<Token> tokens;
    uint index, length, line;

    uint save_index, save_length, save_line;

    readonly string file;


    Lexer(ref string file)
    {
        this.file = file;
        this.tokens = new List<Token>();
        this.line = 1;
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
            RestoreState();
        Utils.TODO("implement Error message in lexer");

        return s;
    }

    Status LexDirector()
    {
        char c = CurrentChar(), p = PeekChar();

        length = 1; // reset length

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
                        length++;
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
                        length++;
                        return AddToken(TknType.GreaterEql);
                    }
                    else if (p == '>')
                    {
                        // '>>'
                        length++;
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
                        length++;
                        return AddToken(TknType.LessEql);
                    }
                    else if (p == '<')
                    {
                        // '<<'
                        length++;
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
                        length++;
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
                        length++;
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
                        length++;
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
                        length++;
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
                        length++;
                        return AddToken(TknType.DivEqual);
                    }
                    else if (p == '/')
                    {
                        // '// single line comments'
                        while (IsNotEOF() && CurrentChar() != '\n')
                            Advance();
                        return Status.Success;
                    }
                    // TODO: Multiline comments
                    // '/'
                    return AddToken(TknType.Div);
                }
            case '!':
                {
                    if (p == '=')
                    {
                        // '!='
                        length++;
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
                case 'x': return LexHexInt();
                case 'b': return LexBinaryInt();
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

        if (length > 0x80)
        {
            Utils.TODO("Handle lexing large number literals");
            return Status.Failure;
        }

        return AddToken(reached_dot ? TknType.FloatLiteral : TknType.IntegerLiteral);
    }

    Status LexHexInt()
    {
        Utils.TODO("Lex Hexadecimal integer literals");
        return Status.Failure;
    }

    Status LexBinaryInt()
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
                line++;
            }
            else { break; }
        }
    }

    void Advance() { ++index; }
    void AdvanceWithLength() { ++index; ++length; }

    void SaveState()
    {
        save_index = index;
        save_length = length;
        save_line = line;
    }

    void RestoreState()
    {
        index = save_index;
        length = save_length;
        line = save_line;
    }


    // NOTE: casted are safe within range due to length check during ReadFile 
    char CurrentChar() { return file[(int)index]; }
    char PeekChar() { return file[(int)index + 1]; }
    char PastChar() { return file[(int)index - 1]; }
    // is not end of file
    bool IsNotEOF() { return index < (uint)file.Length; }

    Status AddToken(TknType type)
    {
        tokens.Add(new Token(index, length, line, type));
        index += length;
        return Status.Success;
    }
}