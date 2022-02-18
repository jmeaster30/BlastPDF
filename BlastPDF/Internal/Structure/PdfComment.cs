namespace BlastPDF.Internal.Structure;

public class PdfComment : PdfObject
{
    public Token Comment { get; set; }

    public PdfComment(Token comment) : base(PdfObjectType.COMMENT)
    {
        Comment = comment;
    }
}