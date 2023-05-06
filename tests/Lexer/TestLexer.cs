using Xunit;

namespace A7TestLexer
{
    public class TestLexer
    {
        [Fact]
        public void TestDigitNumber()
        {
            string s = Utils.PrepareStrForParsing("12 12 0xabc 0b10101");
            A7.Lexer lex = new A7.Lexer("test", ref s);
            var st = lex.Lex();
            Assert.Equal(Status.Done, st);
            Assert.Equal(TokensCountTest(4), lex.GetTokens().Count());

            // didnt convert to loop due to EOT type tokens
            Assert.Equal(A7.TknType.IntegerLiteral, lex.GetTokens()[0].type);
            Assert.Equal(A7.TknType.IntegerLiteral, lex.GetTokens()[1].type);
            Assert.Equal(A7.TknType.IntegerLiteral, lex.GetTokens()[2].type);
            Assert.Equal(A7.TknType.IntegerLiteral, lex.GetTokens()[3].type);
        }


        int TokensCountTest(int i)
        {
            return Utils.NULL_TERMINATORS_COUNT_PASSES + i;
        }
    }
}
