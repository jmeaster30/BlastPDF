namespace BlastPDF.Internal.Structure.Objects;

public class PdfTrailer : PdfObject
{
    public PdfDictionary Dictionary { get; }

    public PdfStartXref StartXref { get; }

    public PdfTrailer(PdfDictionary dictionary, PdfStartXref startXref) : base(PdfObjectType.TRAILER)
    {
        Dictionary = dictionary;
        StartXref = startXref;
    }
}