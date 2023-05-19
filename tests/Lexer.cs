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
        string s = Utilities.PrepareStrForParsing("12 1 1231_2342314 0234 0xab_c 0b1_0101");
        Lexer lex = new Lexer("test", ref s);
        var st = lex.Lex();
        Assert.Equal(Status.Done, st);

        // didnt convert to loop due to EOT type tokens
        Assert.Equal(TknType.IntegerLiteral, lex.GetTokens()[0].type);
        Assert.Equal(TknType.IntegerLiteral, lex.GetTokens()[1].type);
        Assert.Equal(TknType.IntegerLiteral, lex.GetTokens()[2].type);
        Assert.Equal(TknType.IntegerLiteral, lex.GetTokens()[3].type);
        Assert.Equal(TknType.IntegerLiteral, lex.GetTokens()[4].type);
        Assert.Equal(TknType.IntegerLiteral, lex.GetTokens()[5].type);
    }

    [Fact]
    public void TestChars()
    {
        string s = "'\n' 'd' // single chars";
        s = Utilities.PrepareStrForParsing(s);
        Lexer lex = new Lexer("test", ref s);
        var st = lex.Lex();
        Assert.Equal(Status.Done, st);
        // didnt convert to loop due to EOT type tokens
        Assert.Equal(TknType.CharLiteral, lex.GetTokens()[0].type);
        Assert.Equal(TknType.CharLiteral, lex.GetTokens()[1].type);
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
        s = Utilities.PrepareStrForParsing(s);
        Lexer lex = new Lexer("test", ref s);
        var st = lex.Lex();
        Assert.Equal(Status.Done, st);
        // didnt convert to loop due to EOT type tokens
        Assert.Equal(TknType.StringLiteral, lex.GetTokens()[0].type);
        Assert.Equal(TknType.StringLiteral, lex.GetTokens()[1].type);
    }
}
