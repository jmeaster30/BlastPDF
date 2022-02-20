namespace BlastPDF.Internal
{
  public enum TokenType
  {
    EOF, REGULAR, DELIMITER, WHITESPACE
  }

  public class Token
  {
    public TokenType Type { get; }
    public char Lexeme { get; }
    
    public Token(TokenType type, char lexeme) {
      Type = type;
      Lexeme = lexeme;
    }
  }
}