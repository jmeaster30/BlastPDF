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
    Namespace,
    Import,
    Variable,
    Title,
    CreationDate,
    Author,
    Load,
    Font,
    Image,
    
    Size,
    Name,

    Dot,
    Inch,
    Percent,
    
    Page,
    Single,
    Layout,
    Header,
    Body,
    Footer,
    
    Width,
    Height,
    Margin,
    Left,
    Right,
    Up,
    Down,
    All,
    
    Text,
    Rise,
    Space,
    Word,
    Char,
    Leading,
    Transform,
    Offset,
    Translate,
    Scale,
    Rotate,
    Skew,
    RenderMode,
    
    Fill,
    Stroke,
    FillStroke,
    Invisible,
    FillClip,
    StrokeClip,
    FillStrokeClip,
    Clip,
    
    If,
    Then,
    Else,
    Loop,
    In,
    End,

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
