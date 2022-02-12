using System;
using Xunit;
using BlastPDF.Internal;

namespace BlastPDF.Test.Internal
{
    public class LexerTests
    {
        [Fact]
        public void CheckEmptyStringEof()
        {
            var lexer = Lexer.FromString("");
            var token = lexer.GetNextToken();

            Assert.True(token.Type == TokenType.EOF, $"Expected TokenType.EOF and got TokenType.{token.Type}");
        }

        [Fact]
        public void CheckSingleSpaceWhitespace()
        {
            var lexer = Lexer.FromString(" ");
            var token = lexer.GetNextToken();

            Assert.True(token.Type == TokenType.WHITESPACE, $"Expected TokenType.WHITESPACE and got TokenType.{token.Type}");
            Assert.True(token.Lexeme == " ", $"Expected the lexeme ' ' but got the lexeme '{token.Lexeme}'");
        }

        [Fact]
        public void CheckMultipleWhitespace()
        {
            var lexer = Lexer.FromString(" \t   \t");
            var token = lexer.GetNextToken();
            var eof = lexer.GetNextToken();

            Assert.True(token.Type == TokenType.WHITESPACE, $"Expected TokenType.WHITESPACE and got TokenType.{token.Type}");
            Assert.True(token.Lexeme == " \t   \t", $"Expected the lexeme ' \\t   \\t' but got the lexeme '{token.Lexeme}'");
            Assert.True(eof.Type == TokenType.EOF, $"Expected TokenType.EOF and got TokenType.{eof.Type}");
        }
        [Fact]
        public void CheckMultipleWhitespaceWithNewline()
        {
            var lexer = Lexer.FromString(" \t   \n\t");
            var ws1 = lexer.GetNextToken();
            var newline = lexer.GetNextToken();
            var ws2 = lexer.GetNextToken();
            var eof = lexer.GetNextToken();

            Assert.True(ws1.Type == TokenType.WHITESPACE, $"Expected TokenType.WHITESPACE and got TokenType.{ws1.Type}");
            Assert.True(ws1.Lexeme == " \t   ", $"Expected the lexeme ' \\t   ' but got the lexeme '{ws1.Lexeme}'");

            Assert.True(newline.Type == TokenType.EOL, $"Expected TokenType.EOL and got TokenType.{newline.Type}");
            Assert.True(newline.Lexeme == "\n", $"Expected the lexeme '\\n' but got the lexeme '{newline.Lexeme}'");

            Assert.True(ws2.Type == TokenType.WHITESPACE, $"Expected TokenType.WHITESPACE and got TokenType.{ws2.Type}");
            Assert.True(ws2.Lexeme == "\t", $"Expected the lexeme '\\t' but got the lexeme '{ws2.Lexeme}'");

            Assert.True(eof.Type == TokenType.EOF, $"Expected TokenType.EOF and got TokenType.{eof.Type}");
        }

        [Fact]
        public void CheckLineFeed()
        {
            var lexer = Lexer.FromString("\n");
            var token = lexer.GetNextToken();
            var eof = lexer.GetNextToken();

            Assert.True(token.Type == TokenType.EOL, $"Expected TokenType.EOL and got TokenType.{token.Type}");
            Assert.True(token.Lexeme == "\n", $"Expected the lexeme '\n' but got the lexeme '{token.Lexeme}'");
            Assert.True(eof.Type == TokenType.EOF, $"Expected TokenType.EOF and got TokenType.{eof.Type}");
        }

        [Fact]
        public void CheckCarriageReturn()
        {
            var lexer = Lexer.FromString("\r");
            var token = lexer.GetNextToken();
            var eof = lexer.GetNextToken();

            Assert.True(token.Type == TokenType.EOL, $"Expected TokenType.EOL and got TokenType.{token.Type}");
            Assert.True(token.Lexeme == "\r", $"Expected the lexeme '\n' but got the lexeme '{token.Lexeme}'");
            Assert.True(eof.Type == TokenType.EOF, $"Expected TokenType.EOF and got TokenType.{eof.Type}");
        }

        [Fact]
        public void CheckCarriageReturnLineFeed()
        {
            var lexer = Lexer.FromString("\r\r\n");
            var cr = lexer.GetNextToken();
            var crlf = lexer.GetNextToken();
            var eof = lexer.GetNextToken();

            Assert.True(cr.Type == TokenType.EOL, $"Expected TokenType.EOL and got TokenType.{cr.Type}");
            Assert.True(cr.Lexeme == "\r", $"Expected the lexeme '\r' but got the lexeme '{cr.Lexeme}'");
            Assert.True(crlf.Type == TokenType.EOL, $"Expected TokenType.EOL and got TokenType.{crlf.Type}");
            Assert.True(crlf.Lexeme == "\r\n", $"Expected the lexeme '\r\n' but got the lexeme '{crlf.Lexeme}'");
            Assert.True(eof.Type == TokenType.EOF, $"Expected TokenType.EOF and got TokenType.{eof.Type}");
        }

        [Fact]
        public void CheckComment()
        {
            var lexer = Lexer.FromString("%this is a test\n");
            var comment = lexer.GetNextToken();
            var lf = lexer.GetNextToken();
            var eof = lexer.GetNextToken();

            Assert.True(comment.Type == TokenType.COMMENT, $"Expected TokenType.COMMENT and got TokenType.{comment.Type}");
            Assert.True(comment.Lexeme == "%this is a test", $"Expected the lexeme '%this is a test' but got the lexeme '{comment.Lexeme}'");
            Assert.True(lf.Type == TokenType.EOL, $"Expected TokenType.EOL and got TokenType.{lf.Type}");
            Assert.True(lf.Lexeme == "\n", $"Expected the lexeme '\n' but got the lexeme '{lf.Lexeme}'");
            Assert.True(eof.Type == TokenType.EOF, $"Expected TokenType.EOF and got TokenType.{eof.Type}");
        }
    }
}
