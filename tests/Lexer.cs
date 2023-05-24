using System.Runtime.CompilerServices;

using Xunit;

using A7.Frontend;
using A7.Utils;

namespace A7Test;

public class TestLexer
{

    // TODO: Add tests for each Token Type
    [Fact]
    public void TestDigitNumber()
    {
        Token[] tkns = LexAString("12 1 1231_2342314 0234 0xab_c 0b1_0101");

        // didnt convert to loop due to EOT type tokens
        Assert.Equal(TknType.IntegerLiteral, tkns[0].type);
        Assert.Equal(TknType.IntegerLiteral, tkns[1].type);
        Assert.Equal(TknType.IntegerLiteral, tkns[2].type);
        Assert.Equal(TknType.IntegerLiteral, tkns[3].type);
        Assert.Equal(TknType.IntegerLiteral, tkns[4].type);
        Assert.Equal(TknType.IntegerLiteral, tkns[5].type);
    }

    [Fact]
    public void TestChars()
    {
        Token[] tkns = LexAString("'\n' 'd' // single chars");

        // didnt convert to loop due to EOT type tokens
        Assert.Equal(TknType.CharLiteral, tkns[0].type);
        Assert.Equal(TknType.CharLiteral, tkns[1].type);
    }

    [Fact]
    public void TestString()
    {
        string s = @"`string
        sdfsdf`  `a huge string asdflkjasd;flkjasdlk;fjasd;lfkjasdlkfjasdl;kfjas;dlkf

        asdfkljas;lkfj;aslkdfj;alskdjf
        addaslkdfkjas;ldkfj;alsdf
        as
        dfaskd;jflaskdjfal;sdfkja;lsdf
        addas;kdlfjlaskdjflaskdjfl;aksdjf;lkajuoqiwehiweuhjrkasdf;alskdasdf

        fadsldkfjasl;kdjf;laskdjfolasdjfopiqwejpriqjweopirqjwepoirjqw;lkrjq l;erk

        Dfasdklfjas;lkdjfl;asdwkjf;laksjdf;lkasjd;lfjkasdf
        ad
        faslkdfjas;lkdfja;sdjfm;alkdjfqweruoprquwerpoqiwerpoiqwperoiujqwprkea
        a
        sdflkasdjf;alsdkf;lkasjdfl;kqjwopeiruqopewiurqpoweiurpoqwieuropqwiuropeiuqwpmxcv;jkgsdf;jgkl;sfdjl;gkjsd;lkfjgl;skdfjg;lsdk
        `
        ";
        Token[] tkns = LexAString(s);

        // didnt convert to loop due to EOT type tokens
        Assert.Equal(TknType.StringLiteral, tkns[0].type);
        Assert.Equal(TknType.StringLiteral, tkns[1].type);
    }

    private Token[] LexAString(string input, [CallerMemberName] string member = "")
    {
        string s = Utilities.PrepareStrForParsing(input);
        Lexer lex = new Lexer(member, ref s);
        var st = lex.Lex();
        Assert.Equal(Status.Done, st);

        return lex.GetTokens();
    }
}
