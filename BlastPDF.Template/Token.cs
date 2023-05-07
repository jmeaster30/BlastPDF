namespace BlastPDF.Template;

public enum TokenType {
    Identifier,
    Comment,
    String,
    EmbeddedExpression,
    LeftParen,
    RightParen,
    LeftCurly,
    RightCurly,
    Colon,
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
