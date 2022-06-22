using System;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfBoolean : PdfObject
{
    public Token Token { get; set; }
    public bool Value { get; set; }
    
    public PdfBoolean(Token token) : base(PdfObjectType.BOOLEAN)
    {
        Token = token;
        Value = token.Lexeme == "true";
    }

    public override void Print()
    {
        Console.Write(Value);
    }
}