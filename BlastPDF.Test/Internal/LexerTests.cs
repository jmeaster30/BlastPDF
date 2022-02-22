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

        private static void CheckError(Lexer lexer, string message)
        {
            var token = lexer.GetToken();
            lexer.ConsumeToken();
            Assert.True(token.Type == TokenType.ERROR, $"Expected TokenType.ERROR and got TokenType.{token.Type}");
            Assert.True(token.ErrorMessage == message, $"Expected the message '{message}' but got the message '{token.ErrorMessage}'");
        }

        private static void CheckTokens(Lexer lexer, IEnumerable<Token> tokens)
        {
            foreach (var expected in tokens)
            {
                var peek = lexer.GetToken();
                if (peek.Type == TokenType.EOF)
                {
                    Assert.True(false, "Found EOF before the end of the expected tokens");
                }

                CheckToken(lexer, expected.Type, expected.Lexeme);
            }
        }

        [Fact]
        public void CheckEmptyStringEof()
        {
            var lexer = Lexer.FromString("");
            CheckEOF(lexer);
        }

        [Fact]
        public void CheckSingleSpaceWhitespace()
        {
            var lexer = Lexer.FromString(" ");
            CheckToken(lexer, TokenType.WHITESPACE, " ");
        }

        [Fact]
        public void CheckMultipleWhitespace()
        {
            var lexer = Lexer.FromString(" \t   \t");
            CheckToken(lexer, TokenType.WHITESPACE, " \t   \t");
            CheckEOF(lexer);
        }

        [Fact]
        public void CheckMultipleWhitespaceWithNewline()
        {
            var lexer = Lexer.FromString(" \t   \n\t");
            CheckToken(lexer, TokenType.WHITESPACE, " \t   ");
            CheckToken(lexer, TokenType.EOL, "\n");
            CheckToken(lexer, TokenType.WHITESPACE, "\t");
            CheckEOF(lexer);
        }

        [Fact]
        public void CheckLineFeed()
        {
            var lexer = Lexer.FromString("\n");
            CheckToken(lexer, TokenType.EOL, "\n");
            CheckEOF(lexer);
        }

        [Fact]
        public void CheckCarriageReturn()
        {
            var lexer = Lexer.FromString("\r");
            CheckToken(lexer, TokenType.EOL, "\r");
            CheckEOF(lexer);
        }

        [Fact]
        public void CheckCarriageReturnLineFeed()
        {
            var lexer = Lexer.FromString("\r\r\n");
            CheckToken(lexer, TokenType.EOL, "\r");
            CheckToken(lexer, TokenType.EOL, "\r\n");
            CheckEOF(lexer);
        }

        [Fact]
        public void CheckComment()
        {
            var lexer = Lexer.FromString("%this is a test\n");
            CheckToken(lexer, TokenType.COMMENT, "%this is a test");
            CheckToken(lexer, TokenType.EOL, "\n");
            CheckEOF(lexer);
        }

        [Fact]
        public void NameResolution()
        {
            var lexer = Lexer.FromString("/Name1");
            CheckToken(lexer, TokenType.NAME, "/Name1");
            CheckEOF(lexer);
        }

        [Fact]
        public void NameResolution2()
        {
            var lexer = Lexer.FromString("/ASomewhatLongerName");
            CheckToken(lexer, TokenType.NAME, "/ASomewhatLongerName");
            CheckEOF(lexer);
        }

        [Fact]
        public void NameResolution3()
        {
            var lexer = Lexer.FromString("/paired#28#29parentheses");
            CheckToken(lexer, TokenType.NAME, "/paired#28#29parentheses");
            CheckEOF(lexer);
        }

        [Fact]
        public void NameResolution4()
        {
            var lexer = Lexer.FromString("/A#42");
            CheckToken(lexer, TokenType.NAME, "/A#42");
            CheckEOF(lexer);
        }

        [Fact]
        public void NameResolution5()
        {
            var lexer = Lexer.FromString("/");
            CheckToken(lexer, TokenType.NAME, "/");
            CheckEOF(lexer);
        }

        [Fact]
        public void HexResolution()
        {
            var lexer = Lexer.FromString("<484f574459>");
            CheckToken(lexer, TokenType.HEX, "<484f574459>");
            CheckEOF(lexer);
        }

        [Fact]
        public void HexResolution2()
        {
            var lexer = Lexer.FromString("<3A5>");
            CheckToken(lexer, TokenType.HEX, "<3A5>");
            CheckEOF(lexer);
        }

        [Fact]
        public void HexResolution3()
        {
            var lexer = Lexer.FromString("<48   \t    4 f57  44 5  9>");
            CheckToken(lexer, TokenType.HEX, "<48   \t    4 f57  44 5  9>");
            CheckEOF(lexer);
        }

        [Fact]
        public void HexResolution4()
        {
            var lexer = Lexer.FromString("<48656C6c6F2c20576f726c6421>");
            CheckToken(lexer, TokenType.HEX, "<48656C6c6F2c20576f726c6421>");
            CheckEOF(lexer);
        }

        [Fact]
        public void HexResolution5()
        {
            var lexer = Lexer.FromString("<&laskjdh>");
            CheckError(lexer, "Hex string contains invalid characters.");
            CheckEOF(lexer);
        }

        [Fact]
        public void Numbers()
        {
            var lexer = Lexer.FromString("+.12+-46 .43 8.3876 66 4.+ 0.0  -.23");
            CheckTokens(lexer, new List<Token>
            {
                new(TokenType.REAL, "+.12"),
                new(TokenType.REGULAR, "+"),
                new(TokenType.INTEGER, "-46"),
                new(TokenType.WHITESPACE, " "),
                new(TokenType.REAL, ".43"),
                new(TokenType.WHITESPACE, " "),
                new(TokenType.REAL, "8.3876"),
                new(TokenType.WHITESPACE, " "),
                new(TokenType.INTEGER, "66"),
                new(TokenType.WHITESPACE, " "),
                new(TokenType.REAL, "4."),
                new(TokenType.REGULAR, "+"),
                new(TokenType.WHITESPACE, " "),
                new(TokenType.REAL, "0.0"),
                new(TokenType.WHITESPACE, "  "),
                new(TokenType.REAL, "-.23")
            });
            CheckEOF(lexer);
        }
    }
}
