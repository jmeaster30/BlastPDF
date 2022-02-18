namespace BlastPDF.Internal.Structure;

public class PdfNull : PdfObject
{
    public Token Null { get; set; }

    public PdfNull(Token token) : base(PdfObjectType.NULL)
    {
        Null = token;
    }
}