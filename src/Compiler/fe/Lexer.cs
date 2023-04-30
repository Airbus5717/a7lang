namespace A7
{
    public class Lexer
    {
        public List<Token> m_tokens;
        public int m_index, m_length, m_line;
        public readonly string m_file, filename;
        public ErrKind error;
        // save state variables
        public int save_index, save_length, save_line;

        public Lexer(string filename, ref string file)
        {
            this.m_file = file;
            this.filename = filename;
            this.m_tokens = new List<Token>(100);
            this.m_line = 1;
            this.m_index = 0;
            this.error = ErrKind.UNKNOWN;
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
            {
                Err.LexerErrMsg(this);
            }
            return s;
        }

        Status LexDirector()
        {
            char c = CurrentChar(), p = PeekChar();

            m_length = 0; // reset length

            SkipWhitespace();
            if (Char.IsAsciiDigit(c))
                return LexNumeric();

            if (Char.IsAsciiLetter(c) || c == '_')
                return LexIdentifier();

            if (c == '"')
                return LexString();

            if (c == '\'')
                return LexChar();


            if (c == '@')
                return LexBuiltin();

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
                        // if (p == '.')
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
                        return AddToken(TknType.PlusOperator);
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
                        return AddToken(TknType.MinusOperator);
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
                        return AddToken(TknType.MultOperator);
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
                        // else if (p == '*')
                        // {
                        //     return LexMultiLineComments();
                        // }

                        // '/'
                        return AddToken(TknType.DivOperator);
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
                case ' ':
                case '\n':
                case '\r':
                    // SkipWhitespace() will deal with it
                    return Status.Success;
                case char.MinValue: // '\0'
                    {
                        // NOTE(5717): Extra END OF TOKENS as delimiters
                        for (uint i = 0; i < Utils.NULL_TERMINATORS_COUNT_PASSES; ++i)
                            AddToken(TknType.EOT);
                        return Status.Done;
                    }
                default:
                    break;
            }
            error = ErrKind.UNKNOWN;
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
            AdvanceWithLength();
            while (Char.IsAsciiDigit(CurrentChar()) || CurrentChar() == '.')
            {
                AdvanceWithLength();
                if (CurrentChar() == '.')
                {
                    if (reached_dot) break;
                    reached_dot = true;
                }
            }


            // TODO: handle lex large numerics
            if (m_length > 0x80)
            {
                error = ErrKind.NUM_TOO_LONG;
                return Status.Failure;
            }
            RestoreIndex();
            return AddToken(reached_dot ? TknType.FloatLiteral : TknType.IntegerLiteral);
        }

        Status LexHexIntLiteral()
        {
            AdvanceWithLength(); // '0'
            AdvanceWithLength(); // 'x'

            while (Char.IsAsciiHexDigit(CurrentChar()))
            {
                AdvanceWithLength();
            }


            // TODO: handle lex large numerics
            if (m_length > 20)
            {
                error = ErrKind.NUM_TOO_LONG;
                return Status.Failure;
            }


            return AddToken(TknType.IntegerLiteral);
        }

        Status LexBinaryIntLiteral()
        {
            AdvanceWithLength(); // '0'
            AdvanceWithLength(); // 'b'

            for (char c = CurrentChar(); c == '0' || c == '1'; c = CurrentChar())
                AdvanceWithLength();


            // TODO: handle lex large numerics
            if (m_length > 66)
            {
                error = ErrKind.NUM_TOO_LONG;
                return Status.Failure;
            }

            return AddToken(TknType.IntegerLiteral);
        }


        Status LexBuiltin()
        {
            Utils.TODO("Lex Builtin ");
            return Status.Failure;
        }

        Status LexIdentifier()
        {
            AdvanceWithLength();
            while (Char.IsAsciiLetterOrDigit(CurrentChar()) || CurrentChar() == '_')
            {
                AdvanceWithLength();
            }
            // restore the index for comparing
            RestoreIndex();


            TknType type = TknType.Identifier;

            switch (m_length)
            {
                case 2:
                    {
                        if (m_file.Substring(m_index, m_length).Equals("if"))
                        {
                            type = TknType.IfKeyword;
                            break;
                        }
                    }
                    break;
                default:
                    break;

            }

            if (m_length > 0x80)
            {
                Utils.TODO("Handle long Identifiers");
                return Status.Failure;
            }
            Utils.TODO("implement keyword matcher");

            return AddToken(type);
        }

        Status LexChar()
        {
            Utils.TODO("Lex character literals");
            AdvanceWithLength(); RestoreIndex();
            return Status.Failure;
        }

        Status LexString()
        {
            Utils.TODO("Lex string literals");
            Advance();
            error = ErrKind.STR_NOT_CLOSED;
            return Status.Failure;
        }

        Status LexMultiLineComments()
        {
            // TODO: Fix the Bugs
            Advance(); // '/'
            Advance(); // '*'
            uint deepness = 1;
            char c = CurrentChar(), p = PeekChar();
            while (true)
            {
                if (c == '/' && p == '*') { deepness++; Advance(); Advance(); }
                if (c == '*' && p == '/') { deepness--; Advance(); Advance(); }
                // update
                Advance();
                c = p;
                p = PeekChar();
                if (deepness == 0) break;
                if (p == char.MinValue) break;
            }
            Advance();
            return deepness == 0 ? Status.Success : Status.Failure;
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
        void AdvanceWithLength() { Advance(); ++m_length; }

        //* NOTE(5717): REMOVED THE NEED FOR RESTORE STATE
        // void SaveState()
        // {
        //     save_index = m_index;
        //     save_length = m_length;
        //     save_line = m_line;
        // }

        // void RestoreState()
        // {
        //     m_index = save_index;
        //     m_length = save_length;
        //     m_line = save_line;
        // }


        void RestoreIndex()
        {
            m_index -= m_length;
        }

        char CurrentChar() { return m_file[m_index]; }
        char PeekChar() { return m_file[(m_index + 1)]; }
        char PastChar() { return m_file[(m_index - 1)]; }
        // is not end of file
        bool IsNotEOF() { return (m_index < m_file.Length); }

        bool KeywordCmp(TknType tkn)
        {
            return m_file.Substring(m_index, m_length) ==
                TokenMethods.GetKeywordStr(tkn);
        }

        Status AddToken(TknType type)
        {
            m_tokens.Add(new Token(m_index, m_length, m_line, type));
            m_index += m_length + 1;
            return Status.Success;
        }

    }
}
