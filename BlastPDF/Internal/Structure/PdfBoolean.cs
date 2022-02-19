namespace BlastPDF.Internal.Structure;

public class PdfBoolean : PdfObject
{
    public string Boolean { get; set; }

    public PdfBoolean(bool value) : base(PdfObjectType.BOOLEAN)
    {
        Boolean = value ? "true" : "false";
    }
    public PdfBoolean(string boolean) : base(PdfObjectType.BOOLEAN)
    {
        Boolean = boolean;
    }
}