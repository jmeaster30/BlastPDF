using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BlastPDF.Internal.Structure;

public class PdfNumeric : PdfObject
{
    public bool IsReal { get; set; }
    public string Numeric { get; set; }

    public PdfNumeric(int num) : base(PdfObjectType.NUMERIC)
    {
        Numeric = num.ToString();
        IsReal = false;
    }

    public PdfNumeric(double num) : base(PdfObjectType.NUMERIC)
    {
        Numeric = num.ToString(CultureInfo.InvariantCulture);
        IsReal = true;
    }
    
    public PdfNumeric(IEnumerable<Token> numeric, bool isReal) : base(PdfObjectType.NUMERIC)
    {
        Numeric = string.Join("", numeric.Select(x => x.Lexeme));
        IsReal = isReal;
    }
}