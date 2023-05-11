namespace A7.Frontend;
using A7.Utils;

class Parser
{
    private Lexer lexer { get; }

    Parser(ref Lexer _lexer)
    {
        this.lexer = _lexer;
    }


    Status Parse()
    {
        return Status.Failure;
    }

    Status ParseDirector()
    {
        return Status.Failure;
    }
}
