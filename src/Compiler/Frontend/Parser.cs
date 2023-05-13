namespace A7.Frontend;

using System;
using A7.Utils;

public class Parser
{
    private Token[] m_tokens { get; }
    private int m_index;


    Parser(ref Lexer _lexer)
    {
        this.m_tokens = _lexer.GetTokens();
        this.m_index = 0;
    }


    Status Parse()
    {
        Status s = Status.Failure;

        while (true)
        {
            s = ParseDirector();
            if (s == Status.Success) continue;
            // if status == (Done | Failure) then break
            break;
        }
        if (s == Status.Failure)
        {
            RestoreState();
            Err.ParserErrMsg(this);
        }
        return s;
    }

    private Status ParseDirector()
    {
        // Global Statements
        Status s = Status.Failure;
        bool is_public = CurrentTkn().type == TknType.PubKeyword;
        if (is_public) Advance();
        Token c = CurrentTkn(), n = NextTkn();
        switch (c.type)
        {
            case TknType.Identifier: s = ParseAfterIdentifier(); break;
            default: break;
        }

        return s;
    }


    private Status ParseAfterIdentifier()
    {
        throw new NotImplementedException();
    }

    private Status ParseRecords()
    {
        throw new NotImplementedException();
    }

    private Status ParseVariants()
    {
        throw new NotImplementedException();
    }

    private Status ParseEnums()
    {
        throw new NotImplementedException();
    }

    private Status ParseFunctions()
    {
        throw new NotImplementedException();
    }

    private Status ParseImports()
    {
        throw new NotImplementedException();
    }

    private void RestoreState()
    {
        throw new NotImplementedException();
    }

    private Token CurrentTkn()
    {
        return m_tokens[m_index];
    }

    private Token NextTkn()
    {
        return m_tokens[(m_index + 1)];
    }

    private Token PrevTkn()
    {
        return m_tokens[(m_index - 1)];
    }

    private void Advance() { ++m_index; }
}
