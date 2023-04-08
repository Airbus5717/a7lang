using Xunit;

namespace A7TestLexer
{
    public class TestLexer
    {
        [Fact]
        public void TestDigitNumber()
        {
            string s = Utils.prepareStrForParsing("12 12 0xabc 0b10101");
            A7.Lexer lex = new A7.Lexer("test", ref s);
            var st = lex.Lex();
            Assert.Equal(Status.Done, st);
            Assert.Equal(TokensCountTest(4), lex.m_tokens.Count);

            // didnt convert to loop due to EOT type tokens
            Assert.Equal(A7.TknType.IntegerLiteral, lex.m_tokens[0].type);
            Assert.Equal(A7.TknType.IntegerLiteral, lex.m_tokens[1].type);
            Assert.Equal(A7.TknType.IntegerLiteral, lex.m_tokens[2].type);
            Assert.Equal(A7.TknType.IntegerLiteral, lex.m_tokens[3].type);
        }


        int TokensCountTest(int i)
        {
            return Utils.NULL_TERMINATORS_COUNT_PASSES + i;
        }
    }
}
