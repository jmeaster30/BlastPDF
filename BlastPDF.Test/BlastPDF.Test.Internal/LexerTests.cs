using System;
using Xunit;
using BlastPDF.Internal;

namespace BlastPDF.Test.Internal
{
    public class LexerTests
    {
        [Fact]
        public void CheckEmptyStringEOF()
        {
            Lexer lexer = Lexer.FromString("");

            var token = lexer.GetNextToken();

            Assert.True(token.Type == TokenType.EOF, $"Expected TokenType.EOF and got TokenType.{token.Type}");
        }

        [Fact]
        public void CheckSingleSpaceWhitespace()
        {
            Lexer lexer = Lexer.FromString(" ");

            var token = lexer.GetNextToken();

            Assert.True(token.Type == TokenType.WHITESPACE, $"Expected TokenType.WHITESPACE and got TokenType.{token.Type}");
            Assert.True(token.Lexeme == " ", $"Expected the lexeme ' ' but got the lexeme '{token.Lexeme}'");
        }

        [Fact]
        public void CheckLineFeed()
        {
            Lexer lexer = Lexer.FromString("\n");

            var token = lexer.GetNextToken();

            Assert.True(token.Type == TokenType.EOL, $"Expected TokenType.EOL and got TokenType.{token.Type}");
            Assert.True(token.Lexeme == "\n", $"Expected the lexeme '\n' but got the lexeme '{token.Lexeme}'");
        }

        [Fact]
        public void CheckCarriageReturnLineFeed()
        {
            Lexer lexer = Lexer.FromString("\r\r\n");

            var cr = lexer.GetNextToken();
            var crlf = lexer.GetNextToken();
            var eof = lexer.GetNextToken();

            Assert.True(cr.Type == TokenType.EOL, $"Expected TokenType.EOL and got TokenType.{cr.Type}");
            Assert.True(cr.Lexeme == "\r", $"Expected the lexeme '\r' but got the lexeme '{cr.Lexeme}'");
            Assert.True(crlf.Type == TokenType.EOL, $"Expected TokenType.EOL and got TokenType.{crlf.Type}");
            Assert.True(crlf.Lexeme == "\r\n", $"Expected the lexeme '\r\n' but got the lexeme '{crlf.Lexeme}'");
            Assert.True(eof.Type == TokenType.EOF, $"Expected TokenType.EOF and got TokenType.{eof.Type}");
        }
    }
}
