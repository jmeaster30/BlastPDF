using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BlastPDF.Internal.Structure;

public class PdfNumeric : PdfObject
{
    public bool IsReal { get; set; }
    public Token Numeric { get; set; }

    public PdfNumeric(Token numeric) : base(PdfNodeType.NUMERIC)
    {
        Numeric = numeric;
        IsReal = Numeric.Type == TokenType.REAL;
    }

    public override void Print()
    {
        Console.Write(Numeric);
    }
}