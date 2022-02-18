namespace BlastPDF.Internal.Structure;

public class PdfNumeric : PdfObject
{
    public Token Numeric { get; set; }

    public PdfNumeric(Token numeric) : base(PdfObjectType.NUMERIC)
    {
        Numeric = numeric;
    }
}