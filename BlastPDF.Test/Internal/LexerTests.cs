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

        private static void CheckToken(Token token, TokenType type, string lexeme, string resolvedValue)
        {
            Assert.True(token.Type == type, $"Expected TokenType.{type} and got TokenType.{token.Type}");
            Assert.True(token.Lexeme == lexeme, $"Expected the lexeme '{lexeme}' but got the lexeme '{token.Lexeme}'");
            Assert.True(token.ResolvedValue == resolvedValue, $"Expected the resolved value '{resolvedValue}' but got the resolved value '{token.ResolvedValue}'");
        }

        private static void CheckError(Token token, string message)
        {
            Assert.True(token.Type == TokenType.ERROR, $"Expected TokenType.ERROR and got TokenType.{token.Type}");
            Assert.True(token.ErrorMessage == message, $"Expected the message '{message}' but got the message '{token.ErrorMessage}'");
        }

        private static void CheckTokens(Lexer lexer, IEnumerable<Token> tokens)
        {
            foreach (var expected in tokens)
            {
                var actual = lexer.GetNextToken();
                CheckToken(expected, actual.Type, actual.Lexeme, actual.ResolvedValue);
                if (actual.Type == TokenType.EOF)
                {
                    Assert.True(false, "Found EOF before the end of the expected tokens");
                }
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
            var token = lexer.GetNextToken();
            CheckToken(token, TokenType.WHITESPACE, " ");
        }

        [Fact]
        public void CheckMultipleWhitespace()
        {
            var lexer = Lexer.FromString(" \t   \t");
            var token = lexer.GetNextToken();
            CheckToken(token, TokenType.WHITESPACE, " \t   \t");
            CheckEOF(lexer);
        }
        
        [Fact]
        public void CheckMultipleWhitespaceWithNewline()
        {
            var lexer = Lexer.FromString(" \t   \n\t");
            var ws1 = lexer.GetNextToken();
            var newline = lexer.GetNextToken();
            var ws2 = lexer.GetNextToken();
            
            CheckToken(ws1, TokenType.WHITESPACE, " \t   ");
            CheckToken(newline, TokenType.EOL, "\n");
            CheckToken(ws2, TokenType.WHITESPACE, "\t");
            CheckEOF(lexer);
        }

        [Fact]
        public void CheckLineFeed()
        {
            var lexer = Lexer.FromString("\n");
            var token = lexer.GetNextToken();
            CheckToken(token, TokenType.EOL, "\n");
            CheckEOF(lexer);
        }

        [Fact]
        public void CheckCarriageReturn()
        {
            var lexer = Lexer.FromString("\r");
            var token = lexer.GetNextToken();
            CheckToken(token, TokenType.EOL, "\r");
            CheckEOF(lexer);
        }

        [Fact]
        public void CheckCarriageReturnLineFeed()
        {
            var lexer = Lexer.FromString("\r\r\n");
            var cr = lexer.GetNextToken();
            var crlf = lexer.GetNextToken();
            
            CheckToken(cr, TokenType.EOL, "\r");
            CheckToken(crlf, TokenType.EOL, "\r\n");
            CheckEOF(lexer);
        }

        [Fact]
        public void CheckComment()
        {
            var lexer = Lexer.FromString("%this is a test\n");
            var comment = lexer.GetNextToken();
            var lf = lexer.GetNextToken();
            
            CheckToken(comment, TokenType.COMMENT, "%this is a test");
            CheckToken(lf, TokenType.EOL, "\n");
            CheckEOF(lexer);
        }
        
        [Fact]
        public void NameResolution()
        {
            var lexer = Lexer.FromString("/Name1");
            var name = lexer.GetNextToken();
            CheckToken(name, TokenType.NAME, "/Name1", "Name1");
            CheckEOF(lexer);
        }
        
        [Fact]
        public void NameResolution2()
        {
            var lexer = Lexer.FromString("/ASomewhatLongerName");
            var name = lexer.GetNextToken();
            CheckToken(name, TokenType.NAME, "/ASomewhatLongerName", "ASomewhatLongerName");
            CheckEOF(lexer);
        }
        
        [Fact]
        public void NameResolution3()
        {
            var lexer = Lexer.FromString("/paired#28#29parentheses");
            var name = lexer.GetNextToken();
            CheckToken(name, TokenType.NAME, "/paired#28#29parentheses", "paired()parentheses");
            CheckEOF(lexer);
        }
        
        [Fact]
        public void NameResolution4()
        {
            var lexer = Lexer.FromString("/A#42");
            var name = lexer.GetNextToken();
            CheckToken(name, TokenType.NAME, "/A#42", "AB");
            CheckEOF(lexer);
        }
        
        [Fact]
        public void NameResolution5()
        {
            var lexer = Lexer.FromString("/");
            var name = lexer.GetNextToken();
            CheckToken(name, TokenType.NAME, "/", "");
            CheckEOF(lexer);
        }
        
        [Fact]
        public void HexResolution()
        {
            var lexer = Lexer.FromString("<484f574459>");
            var hex = lexer.GetNextToken();
            CheckToken(hex, TokenType.HEX, "<484f574459>", "HOWDY");
            CheckEOF(lexer);
        }
        
        [Fact]
        public void HexResolution2()
        {
            var lexer = Lexer.FromString("<3A5>");
            var hex = lexer.GetNextToken();
            CheckToken(hex, TokenType.HEX, "<3A5>", ":P");
            CheckEOF(lexer);
        }
        
        [Fact]
        public void HexResolution3()
        {
            var lexer = Lexer.FromString("<48   \t    4 f57  44 5  9>");
            var hex = lexer.GetNextToken();
            CheckToken(hex, TokenType.HEX, "<48   \t    4 f57  44 5  9>", "HOWDY");
            CheckEOF(lexer);
        }
        
        [Fact]
        public void HexResolution4()
        {
            var lexer = Lexer.FromString("<48656C6c6F2c20576f726c6421>");
            var hex = lexer.GetNextToken();
            CheckToken(hex, TokenType.HEX, "<48656C6c6F2c20576f726c6421>", "Hello, World!");
            CheckEOF(lexer);
        }
        
        [Fact]
        public void HexResolution5()
        {
            var lexer = Lexer.FromString("<&laskjdh>");
            var hex = lexer.GetNextToken();
            CheckError(hex, "Hex string contains invalid characters.");
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
