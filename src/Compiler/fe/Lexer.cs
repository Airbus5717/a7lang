namespace A7
{
    public class Lexer
    {
        private List<Token> m_tokens;
        private int m_index, m_length, m_line;
        private ErrKind m_error;

        public string m_file { get; }
        public string filename { get; }

        // save state variables
        // for restoring state in error flow
        private int save_index, save_line;

        public Lexer(string filename, ref string file)
        {
            this.m_file = file;
            this.filename = filename;
            this.m_tokens = new List<Token>(100);
            this.m_line = 1;
            this.m_index = 0;
            this.m_error = ErrKind.UNKNOWN;
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
                RestoreState();
                Err.LexerErrMsg(this);
            }
            return s;
        }

        Status LexDirector()
        {
            char c = CurrentChar(), p = PeekChar();
            SaveState();
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

            m_length++;
            switch (c)
            {
                case ';': return AddToken(TknType.Terminator);
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
                case '\t':
                case '\r':
                case '\n':
                    // NOTE: SkipWhitespace() will deal with it, next iteration
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
            m_error = ErrKind.UNKNOWN;
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
                    // case 'o': return LexOctalIntLiteral();
                    default: break;
                }
            }

            bool reached_dot = false;
            AdvanceWithLength();
            while (Char.IsAsciiDigit(CurrentChar()) || CurrentChar() == '.' || CurrentChar() == '_')
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
                m_error = ErrKind.NUM_TOO_LONG;
                return Status.Failure;
            }
            RestoreIndex();
            return AddToken(reached_dot ? TknType.FloatLiteral : TknType.IntegerLiteral);
        }

        Status LexHexIntLiteral()
        {
            AdvanceWithLength(); // '0'
            AdvanceWithLength(); // 'x'

            while (Char.IsAsciiHexDigit(CurrentChar()) || CurrentChar() == '_')
            {
                AdvanceWithLength();
            }


            // TODO: handle lex large numerics
            if (m_length > 20)
            {
                m_error = ErrKind.NUM_TOO_LONG;
                return Status.Failure;
            }


            return AddToken(TknType.IntegerLiteral);
        }

        Status LexBinaryIntLiteral()
        {
            AdvanceWithLength(); // '0'
            AdvanceWithLength(); // 'b'

            for (char c = CurrentChar(); c == '0' || c == '1' || c == '_'; c = CurrentChar())
                AdvanceWithLength();


            // TODO: handle lex large numerics
            if (m_length > 66)
            {
                m_error = ErrKind.NUM_TOO_LONG;
                return Status.Failure;
            }

            return AddToken(TknType.IntegerLiteral);
        }


        Status LexBuiltin()
        {
            Utils.Todo("Lex Builtin implementation");
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
            // TODO: optimize Keyword comparing (profile before optimizing)
            switch (m_length)
            {
                case 2:
                    {
                        if (KeywordCmp(TknType.IfKeyword)) type = TknType.IfKeyword;
                        else if (KeywordCmp(TknType.OrKeyword)) type = TknType.OrKeyword;
                        else if (KeywordCmp(TknType.AsKeyword)) type = TknType.AsKeyword;
                        else if (KeywordCmp(TknType.FnKeyword)) type = TknType.FnKeyword;
                        break;
                    }
                case 3:
                    {
                        if (KeywordCmp(TknType.AndKeyword)) type = TknType.AndKeyword;
                        else if (KeywordCmp(TknType.ForKeyword)) type = TknType.ForKeyword;
                        else if (KeywordCmp(TknType.NewKeyword)) type = TknType.NewKeyword;
                        else if (KeywordCmp(TknType.NilLiteral)) type = TknType.NilLiteral;
                        else if (KeywordCmp(TknType.RetKeyword)) type = TknType.RetKeyword;
                        else if (KeywordCmp(TknType.PubKeyword)) type = TknType.PubKeyword;
                        else if (KeywordCmp(TknType.RefKeyword)) type = TknType.RefKeyword;
                        else if (KeywordCmp(TknType.IntKeyword)) type = TknType.IntKeyword;
                        else if (KeywordCmp(TknType.FltKeyword)) type = TknType.FltKeyword;
                        break;
                    }
                case 4:
                    {
                        if (KeywordCmp(TknType.ElseKeyword)) type = TknType.ElseKeyword;
                        else if (KeywordCmp(TknType.BoolKeyword)) type = TknType.BoolKeyword;
                        else if (KeywordCmp(TknType.CharKeyword)) type = TknType.CharKeyword;
                        else if (KeywordCmp(TknType.EnumKeyword)) type = TknType.EnumKeyword;
                        else if (KeywordCmp(TknType.FallKeyword)) type = TknType.FallKeyword;
                        else if (KeywordCmp(TknType.TrueLiteral)) type = TknType.TrueLiteral;
                        else if (KeywordCmp(TknType.UIntKeyword)) type = TknType.UIntKeyword;
                        break;
                    }
                case 5:
                    {
                        if (KeywordCmp(TknType.BreakKeyword)) type = TknType.BreakKeyword;
                        else if (KeywordCmp(TknType.MatchKeyword)) type = TknType.MatchKeyword;
                        else if (KeywordCmp(TknType.DeferKeyword)) type = TknType.DeferKeyword;
                        else if (KeywordCmp(TknType.FalseLiteral)) type = TknType.FalseLiteral;
                        break;
                    }
                case 6:
                    {
                        if (KeywordCmp(TknType.DeleteKeyword)) type = TknType.DeleteKeyword;
                        else if (KeywordCmp(TknType.ImportKeyword)) type = TknType.ImportKeyword;
                        else if (KeywordCmp(TknType.RecordKeyword)) type = TknType.RecordKeyword;
                        break;
                    }
                case 7:
                    {
                        if (KeywordCmp(TknType.ForEachKeyword)) type = TknType.ForEachKeyword;
                        else if (KeywordCmp(TknType.VariantKeyword)) type = TknType.VariantKeyword;
                        break;
                    }
                default:
                    break;

            }

            if (m_length > 0x80)
            {
                Utils.Todo("Handle long Identifiers");
                return Status.Failure;
            }

            return AddToken(type);
        }

        Status LexChar()
        {
            Utils.Todo("Lex character literals");
            AdvanceWithLength(); RestoreIndex();
            return Status.Failure;
        }

        Status LexString()
        {
            Utils.Todo("Lex string literals");
            Advance();
            m_error = ErrKind.STR_NOT_CLOSED;
            return Status.Failure;
        }

        // Status LexMultiLineComments()
        // {
        //     // TODO: Fix the Bugs
        //     Utils.Todo("implement Lex Multi line comments; Nested comments too");
        //     Advance(); // '/'
        //     Advance(); // '*'
        //     uint deepness = 1;
        //     char c = CurrentChar(), p = PeekChar();
        //     while (true)
        //     {
        //         if (c == '/' && p == '*') { deepness++; Advance(); Advance(); }
        //         if (c == '*' && p == '/') { deepness--; Advance(); Advance(); }
        //         // update
        //         Advance();
        //         c = p;
        //         p = PeekChar();
        //         if (deepness == 0) break;
        //         if (p == char.MinValue) break;
        //     }
        //     Advance();
        //     return deepness == 0 ? Status.Success : Status.Failure;
        // }

        void SkipWhitespace()
        {
            while (true)
            {
                char c = CurrentChar();
                if (c == ' ' || c == '\t') { Advance(); }
                else if (c == '\n')
                {
                    m_line++;
                    AddToken(TknType.Terminator);
                    Advance();
                }
                else { break; }
            }
        }

        void Advance() { ++m_index; }
        void AdvanceWithLength() { ++m_index; ++m_length; }

        //* FOR ERROR HANDLING
        void SaveState()
        {
            save_index = m_index;
            save_line = m_line;
        }

        //* FOR ERROR HANDLING
        void RestoreState()
        {
            m_index = save_index;
            m_line = save_line;
        }

        void RestoreIndex()
        {
            m_index -= m_length;
        }

        char CurrentChar() { return m_file[m_index]; }
        char PeekChar() { return m_file[(m_index + 1)]; }
        char PastChar() { return m_file[(m_index - 1)]; }
        // is not end of file
        bool IsNotEOF() { return (m_index < m_file.Length); }

        bool KeywordCmp(TknType type)
        {
            return m_file.Substring(m_index, m_length) ==
                TokenMethods.GetKeywordStr(type);
        }

        Status AddToken(TknType type)
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
        public ErrKind GetErr() { return m_error; }
    }
}
