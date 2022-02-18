namespace BlastPDF.Internal.Structure;

public class PdfBoolean : PdfObject
{
    public Token Boolean { get; set; }
    public PdfBoolean(Token boolean) : base(PdfObjectType.BOOLEAN)
    {
        Boolean = boolean;
    }
}