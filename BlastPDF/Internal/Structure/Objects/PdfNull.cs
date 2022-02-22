using System;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfNull : PdfObject
{
    public Token Token { get; set; }

    public PdfNull(Token value) : base(PdfNodeType.NULL)
    {
        Token = value;
    }

    public override void Print()
    {
        Console.Write(Token.Lexeme);
    }
}