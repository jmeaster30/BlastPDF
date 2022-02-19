using System.Collections.Generic;
using System.Linq;

namespace BlastPDF.Internal.Structure;

public class PdfLiteralString : PdfObject
{
    public string StringValue { get; set; }
    public string ResolvedValue { get; set; }

    public PdfLiteralString(string value) : base(PdfObjectType.LITERAL_STRING)
    {
        StringValue = value;
        ResolvedValue = StringValue.ResolveLiteralString();
    }
    
    public PdfLiteralString(IEnumerable<Token> value) : base(PdfObjectType.LITERAL_STRING)
    {
        StringValue = string.Join("", value.Select(x => x.Lexeme));
        ResolvedValue = StringValue.ResolveLiteralString();
    }
}