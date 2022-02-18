using System.Collections.Generic;
using Xunit;
using BlastPDF.Internal;

namespace BlastPDF.Test.Internal
{
    public class LexerTests
    {
        private static void CheckEOF(Lexer lexer)
        {
            var token = lexer.GetNextToken();
            Assert.True(token.Type == TokenType.EOF, $"Expected TokenType.EOF and got TokenType.{token.Type}");
        }
        
        private static void CheckToken(Token token, TokenType type, string lexeme)
        {
            Assert.True(token.Type == type, $"Expected TokenType.{type} and got TokenType.{token.Type}");
            Assert.True(token.Lexeme == lexeme, $"Expected the lexeme '{lexeme}' but got the lexeme '{token.Lexeme}'");
        }

        [Fact]
        public void LexTest()
        {
            var lexer = Lexer.FromString("a/%1s  \r\nwo");
            
            CheckToken(lexer.GetNextToken(), TokenType.REGULAR, "a");
            CheckToken(lexer.GetNextToken(), TokenType.DELIMITER, "/");
            CheckToken(lexer.GetNextToken(), TokenType.DELIMITER, "%");
            CheckToken(lexer.GetNextToken(), TokenType.REGULAR, "1");
            CheckToken(lexer.GetNextToken(), TokenType.REGULAR, "s");
            CheckToken(lexer.GetNextToken(), TokenType.WHITESPACE, " ");
            CheckToken(lexer.GetNextToken(), TokenType.WHITESPACE, " ");
            CheckToken(lexer.GetNextToken(), TokenType.WHITESPACE, "\r");
            CheckToken(lexer.GetNextToken(), TokenType.WHITESPACE, "\n");
            CheckToken(lexer.GetNextToken(), TokenType.REGULAR, "w");
            CheckToken(lexer.GetNextToken(), TokenType.REGULAR, "o");
            CheckEOF(lexer);
        }
    }
}
