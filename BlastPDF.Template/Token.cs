namespace BlastPDF.Template;

public enum TokenType {
    Keyword,
    Comment,
    String,
    MultilineString,
    EmbeddedExpression,
    LeftParen,
    RightParen,
    LeftCurly,
    RightCurly,
    Unknown
}

public class Token
{
    public TokenType Type { get; set; }
    public string Lexeme { get; set; } = "";
    public (int, int) Offset { get; set; }
    public (int, int) Column { get; set; }
    public (int, int) Line { get; set; }
}
