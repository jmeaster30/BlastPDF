
namespace BlastPDF.Internal
{
  public enum TokenType
  {
    REGULAR, EOF, EOL, WHITESPACE,
    COMMENT, BOOLEAN, INTEGER, REAL,
    LITERAL, HEX, NAME, NULL,
    ARRAYOPEN, ARRAYCLOSE,
    DICTOPEN, DICTCLOSE,
    STREAM, ENDSTREAM,
    OBJ, ENDOBJ
  }

  public class Token
  {
    public TokenType Type { get; private set; }
    public string Lexeme { get; private set; }
    public string ResolvedValue { get; private set; }

    public Token(TokenType type, string lexeme) {
      Type = type;
      Lexeme = lexeme;
      ResolveTokenValue();
    }

    private void ResolveTokenValue() {
      ResolvedValue = Type switch {
        TokenType.NAME => ResolveName(Lexeme),
        _ => Lexeme,
      };
    }

    private static string ResolveName(string Lexeme) {
      return Lexeme;
    }

    private static string ResolveHexString(string Lexeme) {
      return Lexeme;
    }

    private static string ResolveLiteralString(string Lexeme) {
      return Lexeme;
    }

  }
}