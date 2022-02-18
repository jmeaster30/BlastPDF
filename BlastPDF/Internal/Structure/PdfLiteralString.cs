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
}