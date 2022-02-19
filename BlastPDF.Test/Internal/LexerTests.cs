using System.Collections.Generic;
using Xunit;
using BlastPDF.Internal;

namespace BlastPDF.Test.Internal
{
    public class LexerTests
    {
        private static void CheckEOF(Lexer lexer)
        {
            var token = lexer.GetToken();
            Assert.True(token.Type == TokenType.EOF, $"Expected TokenType.EOF and got TokenType.{token.Type}");
        }
        
        private static void CheckToken(Lexer lexer, TokenType type, string lexeme)
        {
            var token = lexer.GetToken();
            lexer.ConsumeToken();
            Assert.True(token.Type == type, $"Expected TokenType.{type} and got TokenType.{token.Type}");
            Assert.True(token.Lexeme == lexeme, $"Expected the lexeme '{lexeme}' but got the lexeme '{token.Lexeme}'");
        }

        [Fact]
        public void LexTest()
        {
            var lexer = Lexer.FromString("a/%1s  \r\nwo");
            
            CheckToken(lexer, TokenType.REGULAR, "a");
            CheckToken(lexer, TokenType.DELIMITER, "/");
            CheckToken(lexer, TokenType.DELIMITER, "%");
            CheckToken(lexer, TokenType.REGULAR, "1");
            CheckToken(lexer, TokenType.REGULAR, "s");
            CheckToken(lexer, TokenType.WHITESPACE, " ");
            CheckToken(lexer, TokenType.WHITESPACE, " ");
            CheckToken(lexer, TokenType.WHITESPACE, "\r");
            CheckToken(lexer, TokenType.WHITESPACE, "\n");
            CheckToken(lexer, TokenType.REGULAR, "w");
            CheckToken(lexer, TokenType.REGULAR, "o");
            CheckEOF(lexer);
        }
    }
}
