namespace BlastPDF.Internal.Structure;

public class PdfNull : PdfObject
{
    public string Null { get; set; }

    public PdfNull() : base(PdfObjectType.NULL)
    {
        Null = "null";
    }

    public PdfNull(string value) : base(PdfObjectType.NULL)
    {
        Null = value;
    }
}