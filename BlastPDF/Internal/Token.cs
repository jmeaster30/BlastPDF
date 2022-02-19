namespace BlastPDF.Internal
{
  public enum TokenType
  {
    EOF, REGULAR, DELIMITER, WHITESPACE
  }

  public class Token
  {
    public TokenType Type { get; }
    public string Lexeme { get; }
    

    public Token(TokenType type, string lexeme) {
      Type = type;
      Lexeme = lexeme;
    }
  }
}