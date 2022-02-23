using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfNumeric : PdfObject
{
    public bool IsReal { get; set; }
    public Token Numeric { get; set; }

    public PdfNumeric(Token numeric) : base(PdfObjectType.NUMERIC)
    {
        Numeric = numeric;
        IsReal = Numeric.Type == TokenType.REAL;
    }
    
    public override void Print()
    {
        Console.Write(Numeric.Lexeme);
    }
}