using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BlastPDF.Internal.Structure;

public class PdfNumeric : PdfObject
{
    public string Numeric { get; set; }

    public PdfNumeric(int num) : base(PdfObjectType.NUMERIC)
    {
        Numeric = num.ToString();
    }

    public PdfNumeric(double num) : base(PdfObjectType.NUMERIC)
    {
        Numeric = num.ToString(CultureInfo.InvariantCulture);
    }
    
    public PdfNumeric(IEnumerable<Token> numeric) : base(PdfObjectType.NUMERIC)
    {
        Numeric = string.Join("", numeric.Select(x => x.Lexeme));
    }
}