namespace BlastPDF.Template;

public enum TokenType {
    Identifier,
    Comment,
    Number,
    String,
    EmbeddedExpression,
    LeftParen,
    RightParen,
    LeftCurly,
    RightCurly,
    Colon,
    Comma,
    Unknown,
    Eof
}

public class Token
{
    public TokenType Type { get; set; }
    public string Lexeme { get; set; } = "";
    public (int, int) Offset { get; set; }
    public (int, int) Column { get; set; }
    public (int, int) Line { get; set; }
}
