namespace BlastPDF.Internal
{
  public enum TokenType
  {
    ERROR, EOF, EOL, REGULAR, WHITESPACE,
    LITERAL, HEX, INTEGER,
    REAL, BOOLEAN, NULL, NAME, COMMENT,
    ARRAY_OPEN, ARRAY_CLOSE, 
    DICT_OPEN, DICT_CLOSE,
    KEYWORD, OPERATOR, STREAM_CONTENT
  }

  public class Token
  {
    public TokenType Type { get; }
    public string Lexeme { get; }
    public long StartOffset { get; }
    public long EndOffset { get; }
    public string ErrorMessage { get; }

    public Token(TokenType type, string lexeme, long startOffset, long endOffset, string error = "") {
      Type = type;
      Lexeme = lexeme;
      StartOffset = startOffset;
      EndOffset = endOffset;
      ErrorMessage = error;
    }
  }
}