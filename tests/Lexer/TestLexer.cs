using Xunit;

using A7.Frontend;
using A7.Utils;


namespace A7TestLexer
{
    public class TestLexer
    {
        [Fact]
        public void TestDigitNumber()
        {
            string s = Utilities.PrepareStrForParsing("12 12 0xabc 0b10101");
            Lexer lex = new Lexer("test", ref s);
            var st = lex.Lex();
            Assert.Equal(Status.Done, st);
            Assert.Equal(TokensCountTest(4), lex.GetTokens().Count());

            // didnt convert to loop due to EOT type tokens
            Assert.Equal(TknType.IntegerLiteral, lex.GetTokens()[0].type);
            Assert.Equal(TknType.IntegerLiteral, lex.GetTokens()[1].type);
            Assert.Equal(TknType.IntegerLiteral, lex.GetTokens()[2].type);
            Assert.Equal(TknType.IntegerLiteral, lex.GetTokens()[3].type);
        }


        int TokensCountTest(int i)
        {
            return Utilities.NULL_TERMINATORS_COUNT_PASSES + i;
        }
    }
}
